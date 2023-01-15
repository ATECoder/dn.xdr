using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace cc.isr.XDR.EncodingExtensions;

public static class EncodingStreamExtensions
{

    /// <summary>
    /// Encodes (aka "serializes") an <see langword="int"/> value into an XDR stream.
    /// </summary>
    /// <exception cref="XdrException">  Thrown when an XDR error condition occurs. </exception>
    /// <param name="value">    The value to be encoded. </param>
    /// <param name="encoder">  The encoder. </param>
    public static void Encode( this int value, XdrEncodingStreamBase encoder )
    {
        encoder.EncodeInt( value );
    }

    /// <summary>
    /// Encodes (aka "serializes") a fixed length XDR opaque data, which are represented by an array
    /// of <see langword="byte"/> values, and starts at <paramref name="offset"/> with a length of <paramref name="length"/>
    /// into an XDR stream.
    /// </summary>
    /// <remarks>
    /// Because the opaque data are encoded without its length information, the receiver has to know 
    /// how long the opaque data is. The encoded data is always padded to be a multiple of four. 
    /// If the given length is not a multiple of four, zero bytes are used for padding. 
    /// </remarks>
    /// <exception cref="XdrException"> Thrown when an XDR error condition occurs. </exception>
    /// <param name="value">    The opaque value to be encoded in the form of a series of bytes. </param>
    /// <param name="offset">   Start offset in the data. </param>
    /// <param name="length">   the number of bytes to encode. </param>
    /// <param name="encoder">  The encoder. </param>
    public static void EncodeOpaque( this byte[] value, int offset, int length, XdrEncodingStreamBase encoder )
    {
        encoder.EncodeOpaque( value, offset, length );
    }

    /// <summary>
    /// Encodes (aka "serializes") a fixed length XDR opaque data, which are represented by an array
    /// of <see langword="byte"/> values into an XDR stream.
    /// </summary>
    /// <remarks>
    /// Because the opaque data are encoded without its length information, the receiver has to know 
    /// how long the opaque data is. The encoded data is always padded to be a multiple of four. 
    /// If the given length is not a multiple of four, zero bytes are used for padding. 
    /// </remarks>
    /// <exception cref="XdrException"> Thrown when an XDR error condition occurs. </exception>
    /// <param name="value">    The opaque data to be encoded in the form of a series 
    ///                         of <see langword="byte"/>s. </param>
    /// <param name="encoder">  The encoder. </param>
    public static void EncodeOpaque( this byte[] value, XdrEncodingStreamBase encoder )
    {
        encoder.EncodeOpaque( value );
    }

    /// <summary>
    /// Encodes (aka "serializes") a fixed length XDR opaque data, which are represented by an array
    /// of <see langword="byte"/> values with a length of <paramref name="length"/> into an XDR
    /// stream.
    /// </summary>
    /// <remarks>
    /// Because the opaque data are encoded without its length information, the receiver has to know 
    /// how long the opaque data is. The encoded data is always padded to be a multiple of four. 
    /// If the given length is not a multiple of four, zero bytes are used for padding.
    /// </remarks>
    /// <exception cref="XdrException"> Thrown when an XDR error condition occurs. </exception>
    /// <param name="value">    The opaque data to be encoded in the form of a series 
    ///                         of <see langword="byte"/>s. </param>
    /// <param name="length">   the number of bytes to encode. </param>
    /// <param name="encoder">  The encoder. </param>
    public static void EncodeOpaque( this byte[] value, int length, XdrEncodingStreamBase encoder )
    {
        encoder.EncodeOpaque( value, length );
    }

    /// <summary>
    /// Encodes (aka "serializes") a variable-length XDR opaque data, which are represented by an
    /// array of <see langword="byte"/> values.
    /// </summary>
    /// <remarks>
    /// The length of the opaque data is written to the XDR stream, so the receiver does not need to
    /// know the exact length in advance. The length is rounded up to a multiple of 4 and the encoded
    /// is always padded to be a multiple of four to maintain XDR alignment.
    /// </remarks>
    /// <exception cref="XdrException"> Thrown when an XDR error condition occurs. </exception>
    /// <param name="value">    The opaque data to be encoded in the form of a series 
    ///                         of <see langword="byte"/>s. </param>
    /// <param name="encoder">  The encoder. </param>
    public static void EncodeDynamicOpaque( this byte[] value, XdrEncodingStreamBase encoder )
    {
        encoder.EncodeDynamicOpaque( value );
    }

    /// <summary>
    /// Encodes (aka "serializes") a fixed length XDR opaque data, which are represented by an array
    /// of <see langword="char"/> values, and starts at <paramref name="offset"/> with a length of <paramref name="length"/>
    /// into an XDR stream.
    /// </summary>
    /// <remarks>
    /// Because the opaque data are encoded without its length information, the receiver has to know 
    /// how long the opaque data is. The encoded data is always padded to be a multiple of four. 
    /// If the given length is not a multiple of four, zero bytes are used for padding. 
    /// </remarks>
    /// <exception cref="XdrException"> Thrown when an XDR error condition occurs. </exception>
    /// <param name="value">    The opaque data to be encoded in the form of a series 
    ///                         of <see langword="byte"/>s. </param>
    /// <param name="offset">   Start offset in the data. </param>
    /// <param name="length">   the number of bytes to encode. </param>
    /// <param name="encoder">  The encoder. </param>
    public static void EncodeOpaque( this char[] value, int offset, int length, XdrEncodingStreamBase encoder )
    {
        encoder.EncodeOpaque( value, offset, length );
    }

    /// <summary>
    /// Encodes (aka "serializes") a fixed length XDR opaque data, which are represented by an array
    /// of <see langword="char"/> values into an XDR stream.
    /// </summary>
    /// <remarks>
    /// Because the opaque data are encoded without its length information, the receiver has to know 
    /// how long the opaque data is. The encoded data is always padded to be a multiple of four. 
    /// If the given length is not a multiple of four, zero bytes are used for padding. 
    /// </remarks>
    /// <exception cref="XdrException"> Thrown when an XDR error condition occurs. </exception>
    /// <param name="value">    The opaque data to be encoded in the form of a series 
    ///                         of <see langword="char"/>s. </param>
    /// <param name="encoder">  The encoder. </param>
    public static void EncodeOpaque( this char[] value, XdrEncodingStreamBase encoder )
    {
        encoder.EncodeOpaque( value );
    }

    /// <summary>
    /// Encodes (aka "serializes") a variable-length XDR opaque data, which are represented by an
    /// array of <see langword="char"/> values.
    /// </summary>
    /// <remarks>
    /// The length of the opaque data is written to the XDR stream, so the receiver does not need to
    /// know the exact length in advance. The length is rounded up to a multiple of 4 and the encoded
    /// is always padded to be a multiple of four to maintain XDR alignment.
    /// </remarks>
    /// <exception cref="XdrException"> Thrown when an XDR error condition occurs. </exception>
    /// <param name="value">    The opaque data to be encoded in the form of a series 
    ///                         of <see langword="char"/>s. </param>
    /// <param name="encoder">  The encoder. </param>
    public static void EncodeDynamicOpaque( this char[] value, XdrEncodingStreamBase encoder )
    {
        encoder.EncodeDynamicOpaqueChar( value );
    }


    /// <summary>
    /// Encodes (aka "serializes") an array of <see langword="byte"/> values into an XDR stream each
    /// packed into its very own 4 bytes XDR int value.
    /// </summary>
    /// <exception cref="XdrException"> Thrown when an XDR error condition occurs. </exception>
    /// <param name="value">    Byte vector to encode. </param>
    /// <param name="offset">   Start offset in the data. </param>
    /// <param name="length">   of vector to write. </param>
    /// <param name="encoder">  The encoder. </param>
    ///
    public static void Encode( this byte[] value, int offset,  int length, XdrEncodingStreamBase encoder )
    {
        encoder.EncodeByteVector( value, offset, length );
    }

    /// <summary>
    /// Encodes (aka "serializes") an array of <see langword="byte"/> values into an XDR stream each
    /// packed into its very own 4 bytes XDR int value.
    /// </summary>
    /// <exception cref="XdrException"> Thrown when an XDR error condition occurs. </exception>
    /// <param name="value">    Byte vector to encode. </param>
    /// <param name="encoder">  The encoder. </param>
    public static void Encode( this byte[] value, XdrEncodingStreamBase encoder )
    {
        encoder.EncodeByteVector( value, 0, value.Length );
    }

    /// <summary>   Encodes (aka "serializes") a <see langword="byte"/> into an XDR stream. </summary>
    /// <exception cref="XdrException"> Thrown when an XDR error condition occurs. </exception>
    /// <param name="value">    <see langword="byte"/> value to encode. </param>
    /// <param name="encoder">  The encoder. </param>
    public static void Encode( this byte value, XdrEncodingStreamBase encoder )
    {
        encoder.EncodeByte( value );
    }

    /// <summary>   Encodes (aka "serializes") a <see langword="char"/> into an XDR stream. </summary>
    /// <exception cref="XdrException"> Thrown when an XDR error condition occurs. </exception>
    /// <param name="value">    <see langword="char"/> value to encode. </param>
    /// <param name="encoder">  The encoder. </param>
    public static void Encode( this char value, XdrEncodingStreamBase encoder )
    {
        encoder.EncodeChar( value );
    }

    /// <summary>
    /// Encodes (aka "serializes") an <see langword="short"/> value into an XDR stream.
    /// </summary>
    /// <exception cref="XdrException"> Thrown when an XDR error condition occurs. </exception>
    /// <param name="value">    The value to be encoded. </param>
    /// <param name="encoder">  The encoder. </param>
    public static void Encode( this short value, XdrEncodingStreamBase encoder )
    {
        encoder.EncodeShort( value );
    }

    /// <summary>
    /// Encodes (aka "serializes") an <see langword="long"/> value into an XDR stream.
    /// </summary>
    /// <exception cref="XdrException"> Thrown when an XDR error condition occurs. </exception>
    /// <param name="value">    The value to be encoded. </param>
    /// <param name="encoder">  The encoder. </param>
    public static void Encode( this long value, XdrEncodingStreamBase encoder )
    {
        encoder.EncodeLong( value );
    }

    /// <summary>
    /// Encodes (aka "serializes") an <see langword="float"/> value into an XDR stream.
    /// </summary>
    /// <exception cref="XdrException"> Thrown when an XDR error condition occurs. </exception>
    /// <param name="value">    The value to be encoded. </param>
    /// <param name="encoder">  The encoder. </param>
    public static void Encode( this float value, XdrEncodingStreamBase encoder )
    {
        encoder.EncodeFloat( value );
    }

    /// <summary>
    /// Encodes (aka "serializes") an <see langword="double"/> value into an XDR stream.
    /// </summary>
    /// <exception cref="XdrException"> Thrown when an XDR error condition occurs. </exception>
    /// <param name="value">    The value to be encoded. </param>
    /// <param name="encoder">  The encoder. </param>
    public static void Encode( this double value, XdrEncodingStreamBase encoder )
    {
        encoder.EncodeDouble( value );
    }

    /// <summary>
    /// Encodes (aka "serializes") a <see langword="bool"/> value into an XDR stream.
    /// </summary>
    /// <exception cref="XdrException">  Thrown when an XDR error condition occurs. </exception>
    /// <param name="value">    The value to be encoded. </param>
    /// <param name="encoder">  The encoder. </param>
    public static void Encode( this bool value, XdrEncodingStreamBase encoder )
    {
        encoder.EncodeBoolean( value );
    }

    /// <summary>
    /// Encodes (aka "serializes") a <see langword="string"/> value into an XDR stream.
    /// </summary>
    /// <exception cref="XdrException">  Thrown when an XDR error condition occurs. </exception>
    /// <param name="value">    The value to be encoded. </param>
    /// <param name="encoder">  The encoder. </param>
    public static void Encode( this string value, XdrEncodingStreamBase encoder )
    {
        encoder.EncodeString( value );
    }

    /// <summary>
    /// Encodes (aka "serializes") a vector of <see langword="short"/> integers into this XDR stream.
    /// </summary>
    /// <exception cref="XdrException">  Thrown when an XDR error condition occurs. </exception>
    /// <param name="value">    <see langword="short"/> vector to be encoded. </param>
    /// <param name="encoder">  The encoder. </param>
    public static void Encode( this short[] value, XdrEncodingStreamBase encoder )
    {
        encoder.EncodeShortVector( value, 0, value.Length );
    }

    /// <summary>
    /// Encodes (aka "serializes") a vector of <see langword="short"/> integers into this XDR stream.
    /// </summary>
    /// <exception cref="XdrException">  Thrown when an XDR error condition occurs. </exception>
    /// <param name="value">    <see langword="short"/> vector to be encoded. </param>
    /// <param name="offset">   Start offset in the data. </param>
    /// <param name="length">   of vector to write. </param>
    /// <param name="encoder">  The encoder. </param>
    public static void Encode( this short[] value, int offset, int length, XdrEncodingStreamBase encoder )
    {
        encoder.EncodeShortVector( value, offset, length );
    }

    /// <summary>
    /// Encodes (aka "serializes") a vector of <see langword="int"/> integers into this XDR stream.
    /// </summary>
    /// <exception cref="XdrException">  Thrown when an XDR error condition occurs. </exception>
    /// <param name="value">    <see langword="int"/> vector to be encoded. </param>
    /// <param name="encoder">  The encoder. </param>
    public static void Encode( this int[] value, XdrEncodingStreamBase encoder )
    {
        encoder.EncodeIntVector( value, 0, value.Length );
    }

    /// <summary>
    /// Encodes (aka "serializes") a vector of <see langword="int"/> integers into this XDR stream.
    /// </summary>
    /// <exception cref="XdrException">  Thrown when an XDR error condition occurs. </exception>
    /// <param name="value">    <see langword="int"/> vector to be encoded. </param>
    /// <param name="offset">   Start offset in the data. </param>
    /// <param name="length">   of vector to write. </param>
    /// <param name="encoder">  The encoder. </param>
    public static void Encode( this int[] value, int offset, int length, XdrEncodingStreamBase encoder )
    {
        encoder.EncodeIntVector( value, offset, length );
    }

    /// <summary>
    /// Encodes (aka "serializes") a vector of <see langword="long"/> integers into this XDR stream.
    /// </summary>
    /// <exception cref="XdrException">  Thrown when an XDR error condition occurs. </exception>
    /// <param name="value">    <see langword="long"/> vector to be encoded. </param>
    /// <param name="encoder">  The encoder. </param>
    public static void Encode( this long[] value, XdrEncodingStreamBase encoder )
    {
        encoder.EncodeLongVector( value, 0, value.Length );
    }

    /// <summary>
    /// Encodes (aka "serializes") a vector of <see langword="long"/> integers into this XDR stream.
    /// </summary>
    /// <exception cref="XdrException">  Thrown when an XDR error condition occurs. </exception>
    /// <param name="value">    <see langword="long"/> vector to be encoded. </param>
    /// <param name="offset">   Start offset in the data. </param>
    /// <param name="length">   of vector to write. </param>
    /// <param name="encoder">  The encoder. </param>
    public static void Encode( this long[] value, int offset, int length, XdrEncodingStreamBase encoder )
    {
        encoder.EncodeLongVector( value, offset, length );
    }

    /// <summary>
    /// Encodes (aka "serializes") a vector of <see langword="float"/> integers into this XDR stream.
    /// </summary>
    /// <exception cref="XdrException">  Thrown when an XDR error condition occurs. </exception>
    /// <param name="value">    <see langword="float"/> vector to be encoded. </param>
    /// <param name="encoder">  The encoder. </param>
    public static void Encode( this float[] value, XdrEncodingStreamBase encoder )
    {
        encoder.EncodeFloatVector( value, 0, value.Length );
    }

    /// <summary>
    /// Encodes (aka "serializes") a vector of <see langword="float"/> integers into this XDR stream.
    /// </summary>
    /// <exception cref="XdrException">  Thrown when an XDR error condition occurs. </exception>
    /// <param name="value">    <see langword="float"/> vector to be encoded. </param>
    /// <param name="offset">   Start offset in the data. </param>
    /// <param name="length">   of vector to write. </param>
    /// <param name="encoder">  The encoder. </param>
    public static void Encode( this float[] value, int offset, int length, XdrEncodingStreamBase encoder )
    {
        encoder.EncodeFloatVector( value, offset, length );
    }

    /// <summary>
    /// Encodes (aka "serializes") a vector of <see langword="double"/> integers into this XDR stream.
    /// </summary>
    /// <exception cref="XdrException">  Thrown when an XDR error condition occurs. </exception>
    /// <param name="value">    <see langword="double"/> vector to be encoded. </param>
    /// <param name="encoder">  The encoder. </param>
    public static void Encode( this double[] value, XdrEncodingStreamBase encoder )
    {
        encoder.EncodeDoubleVector( value, 0, value.Length );
    }

    /// <summary>
    /// Encodes (aka "serializes") a vector of <see langword="double"/> integers into this XDR stream.
    /// </summary>
    /// <exception cref="XdrException">  Thrown when an XDR error condition occurs. </exception>
    /// <param name="value">    <see langword="double"/> vector to be encoded. </param>
    /// <param name="offset">   Start offset in the data. </param>
    /// <param name="length">   of vector to write. </param>
    /// <param name="encoder">  The encoder. </param>
    public static void Encode( this double[] value, int offset, int length, XdrEncodingStreamBase encoder )
    {
        encoder.EncodeDoubleVector( value, offset, length );
    }


    /// <summary>
    /// Encodes (aka "serializes") a vector of <see langword="bool"/> integers into this XDR stream.
    /// </summary>
    /// <exception cref="XdrException">  Thrown when an XDR error condition occurs. </exception>
    /// <param name="value">    <see langword="bool"/> vector to be encoded. </param>
    /// <param name="encoder">  The encoder. </param>
    public static void Encode( this bool[] value, XdrEncodingStreamBase encoder )
    {
        encoder.EncodeBooleanVector( value, 0, value.Length );
    }

    /// <summary>
    /// Encodes (aka "serializes") a vector of <see langword="bool"/> integers into this XDR stream.
    /// </summary>
    /// <exception cref="XdrException">  Thrown when an XDR error condition occurs. </exception>
    /// <param name="value">    <see langword="bool"/> vector to be encoded. </param>
    /// <param name="offset">   Start offset in the data. </param>
    /// <param name="length">   of vector to write. </param>
    /// <param name="encoder">  The encoder. </param>
    public static void Encode( this bool[] value, int offset, int length, XdrEncodingStreamBase encoder )
    {
        encoder.EncodeBooleanVector( value, offset, length );
    }

    /// <summary>
    /// Encodes (aka "serializes") a vector of <see langword="string"/> integers into this XDR stream.
    /// </summary>
    /// <exception cref="XdrException">  Thrown when an XDR error condition occurs. </exception>
    /// <param name="value">    <see langword="string"/> vector to be encoded. </param>
    /// <param name="encoder">  The encoder. </param>
    public static void Encode( this string[] value, XdrEncodingStreamBase encoder )
    {
        encoder.EncodeStringVector( value, 0, value.Length );
    }

    /// <summary>
    /// Encodes (aka "serializes") a vector of <see langword="string"/> integers into this XDR stream.
    /// </summary>
    /// <exception cref="XdrException">  Thrown when an XDR error condition occurs. </exception>
    /// <param name="value">    <see langword="string"/> vector to be encoded. </param>
    /// <param name="offset">   Start offset in the data. </param>
    /// <param name="length">   of vector to write. </param>
    /// <param name="encoder">  The encoder. </param>
    public static void Encode( this string[] value, int offset, int length, XdrEncodingStreamBase encoder )
    {
        encoder.EncodeStringVector( value, offset, length );
    }

}
