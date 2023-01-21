using cc.isr.XDR.Logging;
namespace cc.isr.XDR;

/// <summary>   Defines the abstract base class for all encoding XDR streams. </summary>
/// <remarks>
/// An encoding XDR stream receives data consisting of primitive data types and writes it to a 
/// data sink (for instance, network or memory buffer) in the platform-independent XDR format. <para>
/// 
/// Derived classes need to implement the <see cref="EncodeInt(int)"/>,
/// <see cref="EncodeOpaque(byte[])"/> and <see cref="EncodeOpaque(byte[], int, int)"/>. </para><para>
/// 
/// Remote Tea authors: Harald Albrecht, Jay Walters.</para>
/// </remarks>
public abstract class XdrEncodingStreamBase : IDisposable
{

    #region " construction and cleanup "

    /// <summary>
    /// Closes this encoding XDR stream and releases any system resources associated with this stream.
    /// </summary>
    /// <remarks>
    /// The general contract of <see cref="Close()"/> is that it closes the encoding XDR stream. A closed 
    /// XDR stream cannot perform encoding operations and cannot be reopened. <para>
    /// 
    /// The <see cref="XdrEncodingStreamBase.Close()"/> method of <see cref="XdrEncodingStreamBase"/>
    /// does nothing.</para>
    /// </remarks>
    /// <exception cref="XdrException">  Thrown when an XDR error condition occurs. </exception>
    public virtual void Close()
    {
    }

    #region " disposable implementation "

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged
    /// resources.
    /// </summary>
    /// <remarks> 
    /// Takes account of and updates <see cref="IsDisposed"/>.
    /// Encloses <see cref="Dispose(bool)"/> within a try...finaly block.
    /// </remarks>
    public void Dispose()
    {
        if ( this.IsDisposed ) { return; }
        try
        {
            // Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
            this.Dispose( true );

            // uncomment the following line if Finalize() is overridden above.
            GC.SuppressFinalize( this );
        }
        catch ( Exception ex ) { Logger.Writer.LogMemberError("Exception disposing", ex ); }
        finally
        {
            this.IsDisposed = true;
        }
    }

    /// <summary>   Gets or sets a value indicating whether this object is disposed. </summary>
    /// <value> True if this object is disposed, false if not. </value>
    protected bool IsDisposed { get; private set; }

    /// <summary>
    /// Releases the unmanaged resources used by the XdrDecodingStreamBase and optionally releases
    /// the managed resources.
    /// </summary>
    /// <param name="disposing">    True to release both managed and unmanaged resources; false to
    ///                             release only unmanaged resources. </param>
    protected virtual void Dispose( bool disposing )
    {
        if ( disposing )
        {
            // dispose managed state (managed objects)
        }
        // free unmanaged resources and override finalizer
        this.Close();

        // set large fields to null
    }

    /// <summary>   Finalizer. </summary>
    ~XdrEncodingStreamBase()
    {
        if ( this.IsDisposed ) { return; }
        this.Dispose( false );
    }

    #endregion

    #endregion

    #region " defaults "

    /// <summary>   Gets or sets the default size of the buffer. </summary>
    public static int BufferSizeDefault { get; set; } = 8192;

    /// <summary>   Gets or sets the default minimum size of the buffer. </summary>
    public static int MinBufferSizeDefault { get; set; } = 1024;

    /// <summary>   Gets or sets the default encoding. </summary>
    /// <remarks>
    /// The default encoding for VXI-11 is <see cref="Encoding.ASCII"/>, which is a subset of <see cref="Encoding.UTF8"/>
    /// </remarks>
    /// <value> The default encoding. </value>
    public static Encoding EncodingDefault { get; set; } = Encoding.UTF8;

    #endregion

    #region " members "

    /// <summary>
    /// Gets or sets the encoding to use when serializing strings. 
    /// </summary>
    /// <value> The character encoding. </value>
    public Encoding CharacterEncoding { get; set; } = XdrDecodingStreamBase.EncodingDefault;

    #endregion

    #region " actions "

    /// <summary>   Begins encoding a new XDR record. </summary>
    /// <remarks>
    /// This typically involves resetting this encoding XDR stream back into a known state.
    /// </remarks>
    /// <param name="remoteEndPoint">   Indicates the remote end point of the receiver of the XDR
    ///                                 data. This can be(<see langword="null"/>) for XDR streams
    ///                                 connected permanently to a receiver (like in case of TCP/IP
    ///                                 based XDR streams). </param>
    public virtual void BeginEncoding( IPEndPoint remoteEndPoint )
    {
    }

    /// <summary>
    /// Flushes this encoding XDR stream and forces any buffered output <see cref="byte"/>s to be written out.
    /// </summary>
    /// <remarks>
    /// The general contract of <see cref="EndEncoding"/> is that calling it is an indication that the
    /// current record is finished and any <see cref="byte"/>s previously encoded should immediately be written to
    /// their intended destination. <para>
    /// 
    /// The <see cref="XdrEncodingStreamBase.EndEncoding"/> method of <see cref="XdrEncodingStreamBase"/>
    /// does nothing.</para>
    /// </remarks>
    public virtual void EndEncoding()
    {
    }

    #endregion

    #region " encode actions "

    /// <summary>
    /// Encodes (aka "serializes") an <see cref="int"/> value into an XDR stream.
    /// </summary>
    /// <remarks>
    /// An XDR int encapsulates a 32 bits <see cref="int"/>.
    /// This method is one of the basic methods all other methods can rely on.
    /// Because it's so basic, derived classes have to implement it.
    /// </remarks>
    /// <exception cref="XdrException">  Thrown when an XDR error condition occurs. </exception>
    /// <param name="value">    The <see cref="int"/> value to be encoded. </param>
    public abstract void EncodeInt( int value );

    /// <summary>
    /// Encodes (aka "serializes") a fixed-length XDR opaque data, which are represented by an 
    /// array of <see cref="byte"/> values, and starts at <paramref name="offset"/> with a 
    /// length of <paramref name="length"/> into an XDR stream.
    /// </summary>
    /// <remarks>
    /// Because the opaque data are encoded without its length information, the receiver has to know 
    /// how long the opaque data is. The encoded data is always padded to be a multiple of four. 
    /// If the given length is not a multiple of four, zero <see cref="byte"/>s are used for padding. <para>
    /// 
    /// Derived classes must ensure that the proper semantic is maintained.</para>
    /// </remarks>
    /// <exception cref="XdrException"> Thrown when an XDR error condition occurs. </exception>
    /// <param name="value">    The opaque value to be encoded in the form of a series of <see cref="byte"/>s. </param>
    /// <param name="offset">   Start offset in the data. </param>
    /// <param name="length">   the number of <see cref="byte"/>s to encode. </param>
    public abstract void EncodeOpaque( byte[] value, int offset, int length );

    /// <summary>
    /// Encodes (aka "serializes") a fixed-length XDR opaque data, which are represented by an 
    /// array of <see cref="byte"/> values into an XDR stream.
    /// </summary>
    /// <remarks>
    /// Because the opaque data are encoded without its length information, the receiver has to know 
    /// how long the opaque data is. The encoded data is always padded to be a multiple of four. 
    /// If the given length is not a multiple of four, zero <see cref="byte"/>s are used for padding.
    /// </remarks>
    /// <exception cref="XdrException">  Thrown when an XDR error condition occurs. </exception>
    /// <param name="value">    The opaque value to be encoded in the form of a series of <see cref="byte"/>s. </param>
    public void EncodeOpaque( byte[] value )
    {
        this.EncodeOpaque( value, 0, value.Length );
    }

    /// <summary>
    /// Encodes (aka "serializes") a fixed-length XDR opaque data, which are represented by an 
    /// array of <see cref="byte"/> values with a length of <paramref name="length"/> into an XDR stream.
    /// </summary>
    /// <remarks>
    /// Because the opaque data are encoded without its length information, the receiver has to know 
    /// how long the opaque data is. The encoded data is always padded to be a multiple of four. 
    /// If the given length is not a multiple of four, zero <see cref="byte"/>s are used for padding.
    /// </remarks>
    /// <exception cref="ArgumentException">    Thrown when one or more arguments have unsupported or
    ///                                         illegal values. </exception>
    /// <exception cref="XdrException">         Thrown when an XDR error condition occurs. </exception>
    /// <param name="value">    The opaque data to be encoded in the form of a series of <see cref="byte"/>s. </param>
    /// <param name="length">   the number of <see cref="byte"/>s to encode. </param>
    public void EncodeOpaque( byte[] value, int length )
    {
        this.EncodeOpaque( value, 0, length );
    }

    /// <summary>
    /// Encodes (aka "serializes") a variable-length XDR opaque data, which are represented by an array
    /// of <see cref="byte"/> values.
    /// </summary>
    /// <remarks>
    /// The length of the opaque data is written to the XDR stream, so the receiver does not need to
    /// know the exact length in advance. The length is rounded up to a multiple of 4 and the encoded 
    /// is always padded to be a multiple of four to maintain XDR alignment.
    /// </remarks>
    /// <exception cref="XdrException"> Thrown when an XDR error condition occurs. </exception>
    /// <param name="value">    The opaque data to be encoded in the form of a series 
    ///                         of <see cref="byte"/>s. </param>
    public void EncodeDynamicOpaque( byte[] value )
    {
        this.EncodeInt( value.Length );
        this.EncodeOpaque( value );
    }

    /// <summary>
    /// Encodes (aka "serializes") a variable-length XDR opaque data, which are represented by an
    /// array of <see cref="char"/> values.
    /// </summary>
    /// <remarks>
    /// The length of the opaque data is written to the XDR stream, so the receiver does not need to
    /// know the exact length in advance. The length is rounded up to a multiple of 4 and the encoded
    /// is always padded to be a multiple of four to maintain XDR alignment.
    /// </remarks>
    /// <param name="value">    The opaque data to be encoded in the form of a series
    ///                         of <see cref="char"/>s. </param>
    public void EncodeDynamicOpaqueChar( char[] value )
    {
        this.EncodeDynamicOpaque( this.CharacterEncoding.GetBytes( value ) );
    }

    /// <summary>
    /// Encodes (aka "serializes") a fixed-length XDR opaque data, which are represented by an 
    /// array of <see cref="char"/> values, and starts at <paramref name="offset"/> with a 
    /// length of <paramref name="length"/> into an XDR stream.
    /// </summary>
    /// <remarks>
    /// Because the opaque data are encoded without its length information, the receiver has to know 
    /// how long the opaque data is. The encoded data is always padded to be a multiple of four. 
    /// If the given length is not a multiple of four, zero <see cref="byte"/>s are used for padding. <para>
    /// 
    /// Derived classes must ensure that the proper semantic is maintained.</para>
    /// </remarks>
    /// <exception cref="XdrException"> Thrown when an XDR error condition occurs. </exception>
    /// <param name="value">    The opaque value to be encoded in the form of 
    ///                         a series of <see cref="char"/>s. </param>
    /// <param name="offset">   Start offset in the data. </param>
    /// <param name="length">   the number of <see cref="byte"/>s to encode. </param>
    public void EncodeOpaque( char[] value, int offset, int length )
    {
        this.EncodeOpaque( this.CharacterEncoding.GetBytes( value ), offset, length );
    }

    /// <summary>
    /// Encodes (aka "serializes") a fixed-length XDR opaque data, which are represented by an 
    /// array of <see cref="char"/> values into an XDR stream.
    /// </summary>
    /// <remarks>
    /// Because the opaque data are encoded without its length information, the receiver has to know 
    /// how long the opaque data is. The encoded data is always padded to be a multiple of four. 
    /// If the given length is not a multiple of four, zero <see cref="byte"/>s are used for padding.
    /// </remarks>
    /// <exception cref="XdrException">  Thrown when an XDR error condition occurs. </exception>
    /// <param name="value">    The opaque value to be encoded in the form of a series 
    ///                         of <see cref="char"/>s. </param>
    public void EncodeOpaque( char[] value )
    {
        this.EncodeOpaque( value, 0, value.Length );
    }


    /// <summary>
    /// Encodes (aka "serializes") an array of <see cref="byte"/> values into an XDR stream
    /// each packed into its very own 4 <see cref="byte"/>s XDR int value.
    /// </summary>
    /// <remarks>
    /// Each <see cref="byte"/> value is packed into its very own 4 <see cref="byte"/>s XDR int value. <para>
    /// 
    /// <see cref="byte"/> vectors are encoded together with a preceding length value. This way the receiver
    /// doesn't need to know the length of the vector in advance. </para>
    /// </remarks>
    /// <exception cref="XdrException">  Thrown when an XDR error condition occurs. </exception>
    /// <param name="value">    <see cref="byte"/> vector to encode. </param>
    public void EncodeByteVector( byte[] value )
    {
        this.EncodeByteVector( value, 0, value.Length );
    }

    /// <summary>
    /// Encodes (aka "serializes") an array of <see cref="byte"/> values into an XDR stream each
    /// packed into its very own 4 <see cref="byte"/>s XDR int value.
    /// </summary>
    /// <exception cref="XdrException"> Thrown when an XDR error condition occurs. </exception>
    /// <param name="value">    <see cref="byte"/> vector to encode. </param>
    /// <param name="offset">   Start offset in the data. </param>
    /// <param name="length">   of vector to write. </param>
    public void EncodeByteVector( byte[] value, int offset, int length )
    {
        this.EncodeInt( length );
        for ( int i = offset; i < length; ++i )
        {
            this.EncodeInt( ( int ) value[i] );
        }
    }

    /// <summary>   Encodes (aka "serializes") a <see cref="byte"/> into a XDR stream. </summary>
    /// <exception cref="XdrException"> Thrown when an XDR error condition occurs. </exception>
    /// <param name="value">    <see cref="byte"/> value to encode. </param>
    public void EncodeByte( byte value )
    {
        this.EncodeInt( ( int ) value );
    }

    /// <summary>   Encodes (aka "serializes") a <see cref="char"/> into a XDR stream. </summary>
    /// <exception cref="XdrException"> Thrown when an XDR error condition occurs. </exception>
    /// <param name="value">    <see cref="char"/> value to encode. </param>
    public void EncodeChar( char value )
    {
        this.EncodeByte( ( byte ) value );
    }

    /// <summary>
    /// Encodes (aka "serializes") a <see cref="short"/> (which is a 16 bits wide quantity)
    /// into this XDR stream.
    /// </summary>
    /// <exception cref="XdrException">  Thrown when an XDR error condition occurs. </exception>
    /// <param name="value">    <see cref="short"/> value to encode. </param>
    public void EncodeShort( short value )
    {
        this.EncodeInt( ( int ) value );
    }

    /// <summary>
    /// Encodes (aka "serializes") a <see cref="long"/> (which is called a "hyper" in XDR babble
    /// and is 64 bits wide) and write it down this XDR stream.
    /// </summary>
    /// <param name="value">    <see cref="long"/> value to encode. </param>
    public void EncodeLong( long value )
    {

        // Just encode the long (which is called a "hyper" in XDR babble) as
        // two integers in network order, that is: big endian with the high int
        // coming first.

        this.EncodeInt( ( int ) ((value) >> 32) & unchecked(( int ) (0xffffffff)) );
        this.EncodeInt( ( int ) (value & unchecked(( int ) (0xffffffff))) );
    }

    /// <summary>
    /// Encodes (aka "serializes") a <see cref="float"/>, which is a 32 bits wide floating point
    /// quantity into this XDR stream.
    /// </summary>
    /// <exception cref="XdrException">  Thrown when an XDR error condition occurs. </exception>
    /// <param name="value">    <see cref="float"/>  value to encode. </param>
    public void EncodeFloat( float value )
    {
        this.EncodeInt( BitConverter.ToInt32( BitConverter.GetBytes( value ), 0 ) );
    }

    /// <summary>
    /// Encodes (aka "serializes") a <see cref="double"/>, which is a 64 bits wide floating point
    /// quantity into this XDR stream.
    /// </summary>
    /// <exception cref="XdrException">  Thrown when an XDR error condition occurs. </exception>
    /// <param name="value">    <see cref="double"/>  value to encode. </param>
    public void EncodeDouble( double value )
    {
        this.EncodeLong( BitConverter.DoubleToInt64Bits( value ) );
    }

    /// <summary>   Encodes (aka "serializes") a <see cref="bool"/> into this XDR stream. </summary>
    /// <exception cref="XdrException">  Thrown when an XDR error condition occurs. </exception>
    /// <param name="value">    <see cref="bool"/> value to be encoded. </param>
    public void EncodeBoolean( bool value )
    {
        this.EncodeInt( value ? 1 : 0 );
    }

    /// <summary>   Encodes (aka "serializes") a <see cref="string"/> into this XDR stream. </summary>
    /// <exception cref="XdrException">  Thrown when an XDR error condition occurs. </exception>
    /// <param name="value">    <see cref="string"/> value to be encoded. </param>
    public void EncodeString( string value )
    {
        this.EncodeDynamicOpaque( this.CharacterEncoding.GetBytes( value ) );
    }

    /// <summary>
    /// Encodes (aka "serializes") a vector of <see cref="short"/> integers into this XDR stream.
    /// </summary>
    /// <exception cref="XdrException">  Thrown when an XDR error condition occurs. </exception>
    /// <param name="value">    <see cref="short"/> vector to be encoded. </param>
    public void EncodeShortVector( short[] value )
    {
        int length = value.Length;
        this.EncodeInt( length );
        this.EncodeShortVector( value, 0, value.Length );
    }

    /// <summary>
    /// Encodes (aka "serializes") a vector of <see cref="short"/> integers into this XDR stream.
    /// </summary>
    /// <exception cref="XdrException">  Thrown when an XDR error condition occurs. </exception>
    /// <param name="value">    <see cref="short"/> vector to be encoded. </param>
    /// <param name="offset">   Start offset in the data. </param>
    /// <param name="length">   of vector to write. This parameter is used as a sanity check. </param>
    public void EncodeShortVector( short[] value, int offset, int length )
    {
        this.EncodeInt( length );
        for ( int i = offset; i < length; i++ )
        {
            this.EncodeShort( value[i] );
        }
    }

    /// <summary>
    /// Encodes (aka "serializes") a vector of <see cref="int"/>'s into this XDR stream.
    /// </summary>
    /// <exception cref="XdrException">  Thrown when an XDR error condition occurs. </exception>
    /// <param name="value">    <see cref="int"/> vector to be encoded. </param>
    public void EncodeIntVector( int[] value )
    {
        this.EncodeIntVector( value, 0, value.Length );
    }

    /// <summary>
    /// Encodes (aka "serializes") a vector of <see cref="int"/>'s and writes it down this XDR
    /// stream.
    /// </summary>
    /// <exception cref="XdrException">  Thrown when an XDR error condition occurs. </exception>
    /// <param name="value">    <see cref="int"/> vector to be encoded. </param>
    /// <param name="offset">   Start offset in the data. </param>
    /// <param name="length">   of vector to write. This parameter is used as a sanity check. </param>
    public void EncodeIntVector( int[] value, int offset, int length )
    {
        this.EncodeInt( length );
        for ( int i = offset; i < length; i++ )
        {
            this.EncodeInt( value[i] );
        }
    }

    /// <summary>
    /// Encodes (aka "serializes") a vector of <see cref="long"/> integers into this XDR stream.
    /// </summary>
    /// <exception cref="XdrException">  Thrown when an XDR error condition occurs. </exception>
    /// <param name="value">    <see cref="long"/> vector to be encoded. </param>
    public void EncodeLongVector( long[] value )
    {
        this.EncodeLongVector( value, 0, value.Length );
    }

    /// <summary>
    /// Encodes (aka "serializes") a vector of <see cref="long"/> integers into this XDR stream.
    /// </summary>
    /// <exception cref="XdrException">  Thrown when an XDR error condition occurs. </exception>
    /// <param name="value">    <see cref="long"/> vector to be encoded. </param>
    /// <param name="offset">   Start offset in the data. </param>
    /// <param name="length">   of vector to write. This parameter is used as a sanity check. </param>
    public void EncodeLongVector( long[] value, int offset, int length )
    {
        this.EncodeInt( length );
        for ( int i = offset; i < length; i++ )
        {
            this.EncodeLong( value[i] );
        }
    }

    /// <summary>
    /// Encodes (aka "serializes") a vector of <see cref="float"/>s into this XDR stream.
    /// </summary>
    /// <exception cref="XdrException">  Thrown when an XDR error condition occurs. </exception>
    /// <param name="value">    <see cref="float"/> vector to be encoded. </param>
    public void EncodeFloatVector( float[] value )
    {
        this.EncodeFloatVector( value, 0, value.Length );
    }

    /// <summary>
    /// Encodes (aka "serializes") a vector of <see cref="float"/> into this XDR stream.
    /// </summary>
    /// <exception cref="XdrException">  Thrown when an XDR error condition occurs. </exception>
    /// <param name="value">    <see cref="float"/> vector to be encoded. </param>
    /// <param name="offset">   Start offset in the data. </param>
    /// <param name="length">   of vector to write. This parameter is used as a sanity check. </param>
    public void EncodeFloatVector( float[] value, int offset, int length )
    {
        this.EncodeInt( length );
        for ( int i = offset; i < length; i++ )
        {
            this.EncodeFloat( value[i] );
        }
    }

    /// <summary>
    /// Encodes (aka "serializes") a vector of <see cref="double"/>s into this XDR stream.
    /// </summary>
    /// <exception cref="XdrException">  Thrown when an XDR error condition occurs. </exception>
    /// <param name="value">    <see cref="double"/> vector to be encoded. </param>
    public void EncodeDoubleVector( double[] value )
    {
        int length = value.Length;
        this.EncodeDoubleVector( value, 0, length );
    }

    /// <summary>
    /// Encodes (aka "serializes") a vector of <see cref="double"/>s into this XDR stream.
    /// </summary>
    /// <exception cref="XdrException">  Thrown when an XDR error condition occurs. </exception>
    /// <param name="value">    <see cref="double"/> vector to be encoded. </param>
    /// <param name="offset">   Start offset in the data. </param>
    /// <param name="length">   of vector to write. This parameter is used as a sanity check. </param>
    public void EncodeDoubleVector( double[] value, int offset, int length )
    {
        this.EncodeInt( length );
        for ( int i = offset; i < length; i++ )
        {
            this.EncodeDouble( value[i] );
        }
    }

    /// <summary>
    /// Encodes (aka "serializes") a vector of <see cref="bool"/>s into this XDR stream.
    /// </summary>
    /// <exception cref="XdrException">  Thrown when an XDR error condition occurs. </exception>
    /// <param name="value">    <see cref="bool"/> vector to be encoded. </param>
    public void EncodeBooleanVector( bool[] value )
    {
        int length = value.Length;
        this.EncodeBooleanVector( value, 0, length );
    }

    /// <summary>
    /// Encodes (aka "serializes") a vector of <see cref="bool"/>s into this XDR stream.
    /// </summary>
    /// <exception cref="XdrException">         Thrown when an XDR error condition occurs. </exception>
    /// <param name="value">    <see cref="bool"/> vector to be encoded. </param>
    /// <param name="offset">   Start offset in the data. </param>
    /// <param name="length">   of vector to write. This parameter is used as a sanity check. </param>
    public void EncodeBooleanVector( bool[] value, int offset, int length )
    {
        this.EncodeInt( length );
        for ( int i = offset; i < length; i++ )
        {
            this.EncodeBoolean( value[i] );
        }
    }

    /// <summary>
    /// Encodes (aka "serializes") a vector of <see cref="string"/>s into this XDR stream.
    /// </summary>
    /// <exception cref="XdrException">  Thrown when an XDR error condition occurs. </exception>
    /// <param name="value">    <see cref="string"/> vector to be encoded. </param>
    public void EncodeStringVector( string[] value )
    {
        this.EncodeStringVector( value, 0, value.Length );
    }

    /// <summary>
    /// Encodes (aka "serializes") a vector of <see cref="string"/>s and writes it down this XDR
    /// stream.
    /// </summary>
    /// <exception cref="XdrException">  Thrown when an XDR error condition occurs. </exception>
    /// <exception cref="ArgumentException">    Thrown if the length of the vector does not match the
    ///                                         specified length. </exception>
    /// <param name="value">    <see cref="string"/> vector to be encoded. </param>
    /// <param name="offset">   Start offset in the data. </param>
    /// <param name="length">   of vector to write. This parameter is used as a sanity check. </param>
    public void EncodeStringVector( string[] value, int offset, int length )
    {
        this.EncodeInt( length );
        for ( int i = offset; i < length; i++ )
        {
            this.EncodeString( value[i] );
        }
    }

    #endregion

}
