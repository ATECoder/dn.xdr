
namespace cc.isr.XDR;

/// <summary>   Defines the abstract base class for all encoding XDR streams. </summary>
/// <remarks>
/// An encoding XDR stream receives data consisting of primitive data types and writes it to a 
/// data sink (for instance, network or memory buffer) in the platform-independent XDR format. <para>
/// Derived classes need to implement the <see cref="EncodeInt(int)"/>,
/// <see cref="EncodeOpaque(byte[])"/> and <see cref="EncodeOpaque(byte[], int, int)"/>. </para><para>
/// Remote Tea authors: Harald Albrecht, Jay Walters.</para>
/// </remarks>
public abstract class XdrEncodingStreamBase
{

    /// <summary>   (Immutable) size of the default buffer. </summary>
    public const int DefaultBufferSize = 8192;

    /// <summary>   (Immutable) the minimum size of the buffer. </summary>
    public const int MinBufferSize = 1024;

    /// <summary>   Begins encoding a new XDR record. </summary>
    /// <remarks>
    /// This typically involves resetting this encoding XDR stream back into a known state.
    /// </remarks>
    /// <param name="receiverAddress">  Indicates the receiver of the XDR data. This can be 
    ///                                 <see langword="null"/> for XDR streams connected permanently to a
    ///                                 receiver (like in case of TCP/IP based XDR streams). </param>
    /// <param name="receiverPort">     Port number of the receiver. </param>
    ///
    /// <exception cref="XdrException">             Thrown when an XDR error condition occurs. </exception>
    /// <exception cref="System.IO.IOException">    Thrown when an I/O error condition occurs. </exception>
    public virtual void BeginEncoding( IPAddress receiverAddress, int receiverPort )
    {
    }

    /// <summary>
    /// Flushes this encoding XDR stream and forces any buffered output bytes to be written out.
    /// </summary>
    /// <remarks>
    /// The general contract of <see cref="EndEncoding"/> is that calling it is an indication that the
    /// current record is finished and any bytes previously encoded should immediately be written to
    /// their intended destination. <para>
    /// The <see cref="XdrEncodingStreamBase.EndEncoding"/> method of <see cref="XdrEncodingStreamBase"/>
    /// does nothing.</para>
    /// </remarks>
    ///
    /// <exception cref="XdrException">             Thrown when an XDR error condition occurs. </exception>
    /// <exception cref="System.IO.IOException">    Thrown when an I/O error condition occurs. </exception>
    public virtual void EndEncoding()
    {
    }

    /// <summary>
    /// Closes this encoding XDR stream and releases any system resources associated with this stream.
    /// </summary>
    /// <remarks>
    /// The general contract of <see cref="Close()"/> is that it closes the encoding XDR stream. A closed 
    /// XDR stream cannot perform encoding operations and cannot be reopened. <para>
    /// The <see cref="XdrEncodingStreamBase.Close()"/> method of <see cref="XdrEncodingStreamBase"/>
    /// does nothing.</para>
    /// </remarks>
    ///
    /// <exception cref="XdrException">             Thrown when an XDR error condition occurs. </exception>
    /// <exception cref="System.IO.IOException">    Thrown when an I/O error condition occurs. </exception>
    public virtual void Close()
    {
    }

    /// <summary>
    /// Encodes (aka "serializes") an <see cref="int"/> value and writes it down an XDR stream.
    /// </summary>
    /// <remarks>
    /// An XDR int encapsulate a 32 bits <see langword="int"/>.
    /// This method is one of the basic methods all other methods can rely on.
    /// Because it's so basic, derived classes have to implement it.
    /// </remarks>
    /// <exception cref="XdrException">             Thrown when an XDR error condition occurs. </exception>
    /// <exception cref="System.IO.IOException">    Thrown when an I/O error condition occurs. </exception>
    /// <param name="value">    The int value to be encoded. </param>
    public abstract void EncodeInt( int value );

    /// <summary>
    /// Encodes (aka "serializes") a XDR opaque value, which is represented by a vector of byte
    /// values, and starts at <paramref name="offset"/> with a length of <paramref name="length"/>.
    /// </summary>
    /// <remarks>
    /// Only the opaque value is encoded, but no length indication is preceding the opaque value, so the
    /// receiver has to know how long the opaque value will be. The encoded data is always padded to
    /// be a multiple of four. If the given length is not a multiple of four, zero bytes will be used
    /// for padding. <para>
    /// Derived classes must ensure that the proper semantic is maintained.</para>
    /// </remarks>
    /// <exception cref="XdrException">             Thrown when an XDR error condition occurs. </exception>
    /// <exception cref="System.IO.IOException">    Thrown when an I/O error condition occurs. </exception>
    /// <param name="value">    The opaque value to be encoded in the form of a series of bytes. </param>
    /// <param name="offset">   Start offset in the data. </param>
    /// <param name="length">   the number of bytes to encode. </param>
    public abstract void EncodeOpaque( byte[] value, int offset, int length );

    /// <summary>
    /// Encodes (aka "serializes") a XDR opaque value, which is represented by a vector of byte
    /// values.
    /// </summary>
    /// <remarks>
    /// The length of the opaque value is written to the XDR stream, so the receiver does not
    /// need to know the exact length in advance. The encoded data is always padded to be a multiple
    /// of four to maintain XDR alignment.
    /// </remarks>
    /// <param name="value">    The opaque value to be encoded in the form of a series of bytes. </param>
    ///
    /// <exception cref="XdrException">             Thrown when an XDR error condition occurs. </exception>
    /// <exception cref="System.IO.IOException">    Thrown when an I/O error condition occurs. </exception>
    public void EncodeDynamicOpaque( byte[] value )
    {
        this.EncodeInt( value.Length );
        this.EncodeOpaque( value );
    }

    /// <summary>
    /// Encodes (aka "serializes") a XDR opaque value, which is represented by a vector of byte
    /// values.
    /// </summary>
    /// <remarks>
    /// Only the opaque value is encoded, but no length indication is preceding the opaque
    /// value, so the receiver has to know how long the opaque value will be. The encoded data is
    /// always padded to be a multiple of four. If the length of the given byte vector is not a
    /// multiple of four, zero bytes will be used for padding.
    /// </remarks>
    /// <param name="value">    The opaque value to be encoded in the form of a series of bytes. </param>
    ///
    /// <exception cref="XdrException">             Thrown when an XDR error condition occurs. </exception>
    /// <exception cref="System.IO.IOException">    Thrown when an I/O error condition occurs. </exception>
    public void EncodeOpaque( byte[] value )
    {
        this.EncodeOpaque( value, 0, value.Length );
    }

    /// <summary>
    /// Encodes (aka "serializes") a XDR opaque value, which is represented by a vector of byte
    /// values.
    /// </summary>
    /// <remarks>
    /// Only the opaque value is encoded, but no length indication is preceding the opaque
    /// value, so the receiver has to know how long the opaque value will be. The encoded data is
    /// always padded to be a multiple of four. If the length of the given byte vector is not a
    /// multiple of four, zero bytes will be used for padding.
    /// </remarks>
    /// <exception cref="ArgumentException">    if the length of the vector does not match the
    ///                                         specified length. </exception>
    /// <param name="value">    The opaque value to be encoded in the form of a series of bytes. </param>
    /// <param name="length">   of vector to write. This parameter is used as a sanity check. </param>
    ///
    /// <exception cref="XdrException">             Thrown when an XDR error condition occurs. </exception>
    /// <exception cref="System.IO.IOException">    Thrown when an I/O error condition occurs. </exception>
    public void EncodeOpaque( byte[] value, int length )
    {
        if ( value.Length != length )
        {
            throw (new ArgumentException( "array size does not match protocol specification" ));
        }
        this.EncodeOpaque( value, 0, value.Length );
    }

    /// <summary>
    /// Encodes (aka "serializes") a vector of bytes, which is nothing more than a series of octets
    /// (or 8 bits wide bytes), each packed into its very own 4 bytes (XDR int).
    /// </summary>
    /// <remarks>
    /// Byte vectors are encoded together with a preceding length value. This way the receiver doesn't need to know
    /// the length of the vector in advance.
    /// </remarks>
    /// <param name="value">    Byte vector to encode. </param>
    ///
    /// <exception cref="XdrException">             Thrown when an XDR error condition occurs. </exception>
    /// <exception cref="System.IO.IOException">    Thrown when an I/O error condition occurs. </exception>
    public void EncodeByteVector( byte[] value )
    {
        int length = value.Length;
        // well, silly optimizations appear here...
        this.EncodeInt( length );
        this.EncodeByteVector( value, length );
    }

    /// <summary>
    /// Encodes (aka "serializes") a vector of bytes, which is nothing more than a series of octets
    /// (or 8 bits wide bytes), each packed into its very own 4 bytes (XDR int).
    /// </summary>
    /// <exception cref="ArgumentException">    if the length of the vector does not match the
    ///                                         specified length. </exception>
    /// <param name="value">    Byte vector to encode. </param>
    /// <param name="length">   of vector to write. This parameter is used as a sanity check. </param>
    ///
    /// <exception cref="XdrException">             Thrown when an XDR error condition occurs. </exception>
    /// <exception cref="System.IO.IOException">    Thrown when an I/O error condition occurs. </exception>
    public void EncodeByteVector( byte[] value, int length )
    {
        if ( value.Length != length )
        {
            throw (new ArgumentException( "array size does not match protocol specification" ));
        }
        if ( length != 0 )
        {

            // For speed reasons, we do sign extension here, but the higher bits
            // will be removed again when deserializing.

            for ( int i = 0; i < length; ++i )
            {
                this.EncodeInt( ( int ) value[i] );
            }
        }
    }

    /// <summary>   Encodes (aka "serializes") a byte and write it down this XDR stream. </summary>
    /// <param name="value">    Byte value to encode. </param>
    ///
    /// <exception cref="XdrException">             Thrown when an XDR error condition occurs. </exception>
    /// <exception cref="System.IO.IOException">    Thrown when an I/O error condition occurs. </exception>
    public void EncodeByte( byte value )
    {

        // For speed reasons, we do sign extension here, but the higher bits
        // will be removed again when deserializing.

        this.EncodeInt( ( int ) value );
    }

    /// <summary>
    /// Encodes (aka "serializes") a short (which is a 16 bits wide quantity)
    /// and write it down this XDR stream.
    /// </summary>
    /// <param name="value">    Short value to encode. </param>
    ///
    /// <exception cref="XdrException">             Thrown when an XDR error condition occurs. </exception>
    /// <exception cref="System.IO.IOException">    Thrown when an I/O error condition occurs. </exception>
    public void EncodeShort( short value )
    {
        this.EncodeInt( ( int ) value );
    }

    /// <summary>
    /// Encodes (aka "serializes") a long (which is called a "hyper" in XDR babble and is 64 bits
    /// wide) and write it down this XDR stream.
    /// </summary>
    /// <param name="value">    Long value to encode. </param>
    ///
    /// <exception cref="XdrException">             Thrown when an XDR error condition occurs. </exception>
    /// <exception cref="System.IO.IOException">    Thrown when an I/O error condition occurs. </exception>
    public void EncodeLong( long value )
    {

        // Just encode the long (which is called a "hyper" in XDR babble) as
        // two integers in network order, that is: big endian with the high int
        // coming first.

        this.EncodeInt( ( int ) ((value) >> 32) & unchecked(( int ) (0xffffffff)) );
        this.EncodeInt( ( int ) (value & unchecked(( int ) (0xffffffff))) );
    }

    /// <summary>
    /// Encodes (aka "serializes") a float (which is a 32 bits wide floating point quantity) and
    /// write it down this XDR stream.
    /// </summary>
    /// <param name="value">    Float value to encode. </param>
    ///
    /// <exception cref="XdrException">             Thrown when an XDR error condition occurs. </exception>
    /// <exception cref="System.IO.IOException">    Thrown when an I/O error condition occurs. </exception>
    public void EncodeFloat( float value )
    {
        this.EncodeInt( BitConverter.ToInt32( BitConverter.GetBytes( value ), 0 ) );
    }

    /// <summary>
    /// Encodes (aka "serializes") a double (which is a 64 bits wide floating point quantity) and
    /// write it down this XDR stream.
    /// </summary>
    /// <param name="value">    Double value to encode. </param>
    ///
    /// <exception cref="XdrException">             Thrown when an XDR error condition occurs. </exception>
    /// <exception cref="System.IO.IOException">    Thrown when an I/O error condition occurs. </exception>
    public void EncodeDouble( double value )
    {
        this.EncodeLong( BitConverter.DoubleToInt64Bits( value ) );
    }

    /// <summary>   Encodes (aka "serializes") a boolean and writes it down this XDR stream. </summary>
    /// <param name="value">    Boolean value to be encoded. </param>
    ///
    /// <exception cref="XdrException">             Thrown when an XDR error condition occurs. </exception>
    /// <exception cref="System.IO.IOException">    Thrown when an I/O error condition occurs. </exception>
    public void EcodeBoolean( bool value )
    {
        this.EncodeInt( value ? 1 : 0 );
    }

    /// <summary>   Converts this object to a byte array. </summary>
    /// <param name="str">  The string. </param>
    /// <returns>   The given data converted to a byte[]. </returns>
    internal byte[] ToByteArray( string str )
    {
        ASCIIEncoding encoding = new();
        return this.ToByteArray( str, encoding );
    }

    /// <summary>   Converts this object to a byte array. </summary>
    /// <param name="str">      The string. </param>
    /// <param name="encoding"> The encoding. </param>
    /// <returns>   The given data converted to a byte[]. </returns>
    internal byte[] ToByteArray( string str, Encoding encoding )
    {
        return encoding.GetBytes( str );
    }


    /// <summary>   Encodes (aka "serializes") a string and writes it down this XDR stream. </summary>
    /// <param name="value">    String value to be encoded. </param>
    ///
    /// <exception cref="XdrException">             Thrown when an XDR error condition occurs. </exception>
    /// <exception cref="System.IO.IOException">    Thrown when an I/O error condition occurs. </exception>
    public void EncodeString( string value )
    {
        if ( this.CharacterEncoding is not null )
        {
            this.EncodeDynamicOpaque( this.ToByteArray( value, Encoding.GetEncoding( this.CharacterEncoding ) ) );
        }
        else
        {
            this.EncodeDynamicOpaque( this.ToByteArray( value ) );
        }
    }

    /// <summary>
    /// Encodes (aka "serializes") a vector of short integers and writes it down this XDR stream.
    /// </summary>
    /// <param name="value">    short vector to be encoded. </param>
    ///
    /// <exception cref="XdrException">             Thrown when an XDR error condition occurs. </exception>
    /// <exception cref="System.IO.IOException">    Thrown when an I/O error condition occurs. </exception>
    public void EncodeShortVector( short[] value )
    {
        int length = value.Length;
        this.EncodeInt( length );
    }

    /// <summary>
    /// Encodes (aka "serializes") a vector of short integers and writes it down this XDR stream.
    /// </summary>
    /// <exception cref="ArgumentException">    if the length of the vector does not match the
    ///                                         specified length. </exception>
    /// <param name="value">    short vector to be encoded. </param>
    /// <param name="length">   of vector to write. This parameter is used as a sanity check. </param>
    ///
    /// <exception cref="XdrException">             Thrown when an XDR error condition occurs. </exception>
    /// <exception cref="System.IO.IOException">    Thrown when an I/O error condition occurs. </exception>
    public void EncodeShortVector( short[] value, int length )
    {
        if ( value.Length != length )
        {
            throw (new ArgumentException( "array size does not match protocol specification" ));
        }
        for ( int i = 0; i < length; i++ )
        {
            this.EncodeShort( value[i] );
        }
    }

    /// <summary>
    /// Encodes (aka "serializes") a vector of <see cref="int"/>'s and writes it down this XDR stream.
    /// </summary>
    /// <param name="value">    int vector to be encoded. </param>
    ///
    /// <exception cref="XdrException">             Thrown when an XDR error condition occurs. </exception>
    /// <exception cref="System.IO.IOException">    Thrown when an I/O error condition occurs. </exception>
    public void EncodeIntVector( int[] value )
    {
        int length = value.Length;
        this.EncodeInt( length );
        this.EncodeIntVector( value, length );
    }

    /// <summary>
    /// Encodes (aka "serializes") a vector of <see cref="int"/>'s and writes it down this XDR stream.
    /// </summary>
    /// <exception cref="ArgumentException">    if the length of the vector does not match the
    ///                                         specified length. </exception>
    /// <param name="value">    int vector to be encoded. </param>
    /// <param name="length">   of vector to write. This parameter is used as a sanity check. </param>
    ///
    /// <exception cref="XdrException">             Thrown when an XDR error condition occurs. </exception>
    /// <exception cref="System.IO.IOException">    Thrown when an I/O error condition occurs. </exception>
    public void EncodeIntVector( int[] value, int length )
    {
        if ( value.Length != length )
        {
            throw (new ArgumentException( "array size does not match protocol specification" ));
        }
        for ( int i = 0; i < length; i++ )
        {
            this.EncodeInt( value[i] );
        }
    }

    /// <summary>
    /// Encodes (aka "serializes") a vector of long integers and writes it down this XDR stream.
    /// </summary>
    /// <param name="value">    long vector to be encoded. </param>
    ///
    /// <exception cref="XdrException">             Thrown when an XDR error condition occurs. </exception>
    /// <exception cref="System.IO.IOException">    Thrown when an I/O error condition occurs. </exception>
    public void EncodeLongVector( long[] value )
    {
        int length = value.Length;
        this.EncodeInt( length );
        this.EncodeLongVector( value, length );
    }

    /// <summary>
    /// Encodes (aka "serializes") a vector of long integers and writes it down this XDR stream.
    /// </summary>
    /// <exception cref="ArgumentException">    if the length of the vector does not match the
    ///                                         specified length. </exception>
    /// <param name="value">    long vector to be encoded. </param>
    /// <param name="length">   of vector to write. This parameter is used as a sanity check. </param>
    ///
    /// <exception cref="XdrException">             Thrown when an XDR error condition occurs. </exception>
    /// <exception cref="System.IO.IOException">    Thrown when an I/O error condition occurs. </exception>
    public void EncodeLongVector( long[] value, int length )
    {
        if ( value.Length != length )
        {
            throw (new ArgumentException( "array size does not match protocol specification" ));
        }
        for ( int i = 0; i < length; i++ )
        {
            this.EncodeLong( value[i] );
        }
    }

    /// <summary>
    /// Encodes (aka "serializes") a vector of floats and writes it down this XDR stream.
    /// </summary>
    /// <param name="value">    float vector to be encoded. </param>
    ///
    /// <exception cref="XdrException">             Thrown when an XDR error condition occurs. </exception>
    /// <exception cref="System.IO.IOException">    Thrown when an I/O error condition occurs. </exception>
    public void EncodeFloatVector( float[] value )
    {
        int length = value.Length;
        this.EncodeInt( length );
        this.EncodeFloatVector( value, length );
    }

    /// <summary>
    /// Encodes (aka "serializes") a vector of floats and writes it down this XDR stream.
    /// </summary>
    /// <exception cref="ArgumentException">    if the length of the vector does not match the
    ///                                         specified length. </exception>
    /// <param name="value">    float vector to be encoded. </param>
    /// <param name="length">   of vector to write. This parameter is used as a sanity check. </param>
    ///
    /// <exception cref="XdrException">             Thrown when an XDR error condition occurs. </exception>
    /// <exception cref="System.IO.IOException">    Thrown when an I/O error condition occurs. </exception>
    public void EncodeFloatVector( float[] value, int length )
    {
        if ( value.Length != length )
        {
            throw (new ArgumentException( "array size does not match protocol specification" ));
        }
        for ( int i = 0; i < length; i++ )
        {
            this.EncodeFloat( value[i] );
        }
    }

    /// <summary>
    /// Encodes (aka "serializes") a vector of doubles and writes it down this XDR stream.
    /// </summary>
    /// <param name="value">    double vector to be encoded. </param>
    ///
    /// <exception cref="XdrException">             Thrown when an XDR error condition occurs. </exception>
    /// <exception cref="System.IO.IOException">    Thrown when an I/O error condition occurs. </exception>
    public void EncodeDoubleVector( double[] value )
    {
        int length = value.Length;
        this.EncodeInt( length );
        this.EncodeDoubleVector( value, length );
    }

    /// <summary>
    /// Encodes (aka "serializes") a vector of doubles and writes it down this XDR stream.
    /// </summary>
    /// <exception cref="ArgumentException">    if the length of the vector does not match the
    ///                                         specified length. </exception>
    /// <param name="value">    double vector to be encoded. </param>
    /// <param name="length">   of vector to write. This parameter is used as a sanity check. </param>
    ///
    /// <exception cref="XdrException">             Thrown when an XDR error condition occurs. </exception>
    /// <exception cref="System.IO.IOException">    Thrown when an I/O error condition occurs. </exception>
    public void EncodeDoubleVector( double[] value, int length )
    {
        if ( value.Length != length )
        {
            throw (new ArgumentException( "array size does not match protocol specification" ));
        }
        for ( int i = 0; i < length; i++ )
        {
            this.EncodeDouble( value[i] );
        }
    }

    /// <summary>
    /// Encodes (aka "serializes") a vector of booleans and writes it down this XDR stream.
    /// </summary>
    /// <param name="value">    long vector to be encoded. </param>
    ///
    /// <exception cref="XdrException">             Thrown when an XDR error condition occurs. </exception>
    /// <exception cref="System.IO.IOException">    Thrown when an I/O error condition occurs. </exception>
    public void EncodeBooleanVector( bool[] value )
    {
        int length = value.Length;
        this.EncodeInt( length );
        this.EncodeBooleanVector( value, length );
    }

    /// <summary>
    /// Encodes (aka "serializes") a vector of booleans and writes it down this XDR stream.
    /// </summary>
    /// <exception cref="ArgumentException">    if the length of the vector does not match the
    ///                                         specified length. </exception>
    /// <param name="value">    long vector to be encoded. </param>
    /// <param name="length">   of vector to write. This parameter is used as a sanity check. </param>
    ///
    /// <exception cref="XdrException">             Thrown when an XDR error condition occurs. </exception>
    /// <exception cref="System.IO.IOException">    Thrown when an I/O error condition occurs. </exception>
    public void EncodeBooleanVector( bool[] value, int length )
    {
        if ( value.Length != length )
        {
            throw (new ArgumentException( "array size does not match protocol specification" ));
        }
        for ( int i = 0; i < length; i++ )
        {
            this.EcodeBoolean( value[i] );
        }
    }

    /// <summary>
    /// Encodes (aka "serializes") a vector of strings and writes it down this XDR stream.
    /// </summary>
    /// <param name="value">    String vector to be encoded. </param>
    ///
    /// <exception cref="XdrException">             Thrown when an XDR error condition occurs. </exception>
    /// <exception cref="System.IO.IOException">    Thrown when an I/O error condition occurs. </exception>
    public void EncodeStringVector( string[] value )
    {
        int length = value.Length;
        this.EncodeInt( length );
        this.EncodeStringVector( value, length );
    }

    /// <summary>
    /// Encodes (aka "serializes") a vector of strings and writes it down this XDR stream.
    /// </summary>
    /// <exception cref="ArgumentException">    if the length of the vector does not match the
    ///                                         specified length. </exception>
    /// <param name="value">    String vector to be encoded. </param>
    /// <param name="length">   of vector to write. This parameter is used as a sanity check. </param>
    ///
    /// <exception cref="XdrException">             Thrown when an XDR error condition occurs. </exception>
    /// <exception cref="System.IO.IOException">    Thrown when an I/O error condition occurs. </exception>
    public void EncodeStringVector( string[] value, int length )
    {
        if ( value.Length != length )
        {
            throw (new ArgumentException( "array size does not match protocol specification" ));
        }
        for ( int i = 0; i < length; i++ )
        {
            this.EncodeString( value[i] );
        }
    }

    /// <summary>
    /// Gets or sets the encoding to use when serializing strings. If <see langword="null"/>, the system's
    /// default encoding is to be used.
    /// </summary>
    /// <value> The character encoding. </value>
    public string CharacterEncoding { get; set; }
}
