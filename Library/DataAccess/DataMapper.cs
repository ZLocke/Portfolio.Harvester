using System.Data;
using System.Linq.Expressions;
using System.Reflection;

namespace Portfolio.ETL;

/// <summary>
/// Generic converter for turning sql query results into data models.
/// </summary>
/// <typeparam name="T">The data model that matches the rows you will get back from the query</typeparam>
internal class DataMapper<T>
{
    private readonly Func<IDataReader, T> converter;
    private readonly IDataReader dataReader;

    internal DataMapper(IDataReader dataReader)
    {
        this.dataReader = dataReader;
        converter = GetMapFunc();
    }

    /// <summary>
    /// Turns the row currently being read by the datareader into a data model.
    /// </summary>
    /// <returns></returns>
    internal T GetModelFromRow() => converter(dataReader);

    /// <summary>
    /// Conditional expression generator for converting models of type T
    /// </summary>
    /// <returns></returns>
    private Func<IDataReader, T> GetMapFunc()
    {
        List<Expression> expressions = new List<Expression>();

        // The type coming in is a row from a table, represented by IDataRecord
        ParameterExpression input = Expression.Parameter(typeof(IDataRecord));

        // The type coming out is a model, represented by T
        ParameterExpression output = Expression.Variable(typeof(T));
        expressions.Add(Expression.Assign(output, Expression.New(output.Type)));

        // Create an index so that we can iterate through the columns.
        PropertyInfo indexerInfo = typeof(IDataRecord).GetProperty("Item", new[] { typeof(int) });

        // We create an anonymous collection of columns 
        var columns = Enumerable.Range(0, dataReader.FieldCount).Select(i => new { i, name = dataReader.GetName(i) });

        foreach (var column in columns)
        {
#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
            PropertyInfo? columnProperty = output.Type.GetProperty(column.name, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
#pragma warning restore CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
            if (columnProperty == null) continue;

            ConstantExpression columnNameExpr = Expression.Constant(column.i);
            IndexExpression propertyExpr = Expression.MakeIndex(input, indexerInfo, new[] { columnNameExpr });

            // If there is not a value for this column, set the value to the default value for that type.
            ConditionalExpression conversionExpression = Expression.Condition(Expression.Equal(propertyExpr, Expression.Constant(DBNull.Value)),
                Expression.Default(columnProperty.PropertyType),
                Expression.Convert(propertyExpr, columnProperty.PropertyType));

            BinaryExpression bindExpr = Expression.Assign(Expression.Property(output, columnProperty), conversionExpression);

            expressions.Add(bindExpr);
        }

        expressions.Add(output);

        // Compile the function that will convert the row into a model.
        return Expression.Lambda<Func<IDataReader, T>>(Expression.Block(new[] { output }, expressions), input).Compile();
    }
}

