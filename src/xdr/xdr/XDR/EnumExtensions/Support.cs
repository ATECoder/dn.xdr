using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace cc.isr.XDR.EnumExtensions;

/// <summary>   A support class for enum extensions. </summary>
public static class Support
{

    /// <summary>   Gets a description from an Enum. </summary>
    /// <remarks>   2023-01-07. </remarks>
    /// <param name="value">    An enum constant representing the value option. </param>
    /// <returns>   The description. </returns>
    public static string GetDescription( this Enum value )
    {
        return
            value
                .GetType()
                .GetMember( value.ToString() )
                .FirstOrDefault()
                ?.GetCustomAttribute<DescriptionAttribute>()
                ?.Description
            ?? value.ToString();
    }
    /// <summary>   An int extension method that converts a value to a <see cref="XdrExceptionReason"/>. </summary>
    /// <exception cref="ArgumentException">    Thrown when one or more arguments have unsupported or
    ///                                         illegal values. </exception>
    /// <param name="value">    An enum constant representing the enum value. </param>
    /// <returns>   Value as the OncRpcExceptionReason. </returns>
    public static XdrExceptionReason ToExceptionReason( this int value )
    {
        return Enum.IsDefined( typeof( XdrExceptionReason ), value )
            ? ( XdrExceptionReason ) value
            : throw new ArgumentException($"{typeof( int )} value of {value} cannot be cast to {nameof( XdrExceptionReason )}" );
    }

}
