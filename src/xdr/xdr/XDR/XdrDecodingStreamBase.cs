namespace cc.isr.XDR;

/// <summary>   Defines the abstract base class for all decoding XDR streams. </summary>
/// <remarks>
/// A decoding XDR stream returns data and primitive data types which it reads from 
/// a data source (for instance, network or memory buffer) in the platform-independent XDR format. <para>
/// Derived classes need to implement the <see cref="DecodeInt()"/>, <see cref="DecodeOpaque(int)"/> and
/// <see cref="DecodeOpaque(byte[], int, int)"/>. </para> <para>
/// Remote Tea authors: Harald Albrecht, Jay Walters. </para>
/// </remarks>
public abstract class XdrDecodingStreamBase : IDisposable
{
    /// <summary>   (Immutable) size of the default buffer. </summary>
    public const int DefaultBufferSize = 8192;

    /// <summary>   (Immutable) the minimum size of the buffer. </summary>
    public const int MinBufferSize = 1024;

    /// <summary>   Returns the Internet address of the sender of the current XDR data. </summary>
    /// <remarks>
    /// This method should only be called after
    /// <see cref="BeginDecoding()"/>, otherwise it might return stale information.
    /// </remarks>
    /// <returns>   <see cref="IPAddress"/> of the sender of the current XDR data. </returns>
    public abstract IPAddress GetSenderAddress();

    /// <summary>   Returns the port number of the sender of the current XDR data. </summary>
    /// <remarks>
    /// This method should only be called after
    /// <see cref="BeginDecoding()"/>, otherwise it might return stale information.
    /// </remarks>
    /// <returns>   Port number of the sender of the current XDR data. </returns>
    public abstract int GetSenderPort();

    /// <summary>   Initiates decoding of the next XDR record. </summary>
    /// <remarks>
    /// This typically involves filling the internal buffer with the next datagram from the network,
    /// or reading the next chunk of data from a stream-oriented connection. In case of memory-based
    /// communication this might involve waiting for some other process to fill the buffer and signal
    /// availability of new XDR data.
    /// </remarks>
    /// <exception cref="XdrException">             Thrown when an XDR error condition occurs. </exception>
    /// <exception cref="System.IO.IOException">    Thrown when an I/O error condition occurs. </exception>
    public abstract void BeginDecoding();

    /// <summary>   End decoding of the current XDR record. </summary>
    /// <remarks>
    /// The general contract of <see cref="EndDecoding"/> is that calling it is
    /// an indication that the current record is no more interesting to the caller and any allocated 
    /// data for this record can be freed. <para>
    /// The <see cref="XdrDecodingStreamBase.EndDecoding"/> method of <see cref="XdrDecodingStreamBase"/>
    /// does nothing. </para>
    /// </remarks>
    ///
    /// <exception cref="XdrException">             Thrown when an XDR error condition occurs. </exception>
    /// <exception cref="System.IO.IOException">    Thrown when an I/O error condition occurs. </exception>
    public virtual void EndDecoding()
    {
    }

    /// <summary>
    /// Closes this decoding XDR stream and releases any system resources associated with this stream.
    /// </summary>
    /// <remarks>
    /// The general contract of <see cref="Close()"/> is that it closes the decoding XDR stream. A
    /// closed XDR stream cannot perform decoding operations and cannot be reopened. <para>
    /// The <see cref="XdrDecodingStreamBase.Close()"/> method of <see cref="XdrDecodingStreamBase"/>
    /// does nothing.</para>
    /// </remarks>
    ///
    /// <exception cref="XdrException">             Thrown when an XDR error condition occurs. </exception>
    /// <exception cref="System.IO.IOException">    Thrown when an I/O error condition occurs. </exception>
    public virtual void Close()
    {
    }

    /// <summary>   Decodes (aka "deserializes") a "XDR int" value received from an XDR stream. </summary>
    /// <remarks>
    /// An XDR int encapsulate a 32 bits <see langword="int"/>.
    /// This method is one of the basic methods all other methods can rely on.
    /// Because it's so basic, derived classes have to implement it.
    /// </remarks>
    /// <exception cref="XdrException">             Thrown when an XDR error condition occurs. </exception>
    /// <exception cref="System.IO.IOException">    Thrown when an I/O error condition occurs. </exception>
    /// <returns>   The decoded int value. </returns>
    public abstract int DecodeInt();

    /// <summary>
    /// Decodes (aka "deserializes") an opaque value, which is nothing more than a series of octets
    /// (or 8 bits wide bytes).
    /// </summary>
    /// <remarks>
    /// Because the length of the opaque value is given, we don't need to retrieve it from the XDR stream. <para>
    /// Note that this is a basic abstract method, which needs to be
    /// implemented in derived classes.</para>
    /// </remarks>
    /// <exception cref="XdrException">             Thrown when an XDR error condition occurs. </exception>
    /// <exception cref="System.IO.IOException">    Thrown when an I/O error condition occurs. </exception>
    /// <param name="length">   Length of opaque data to decode. </param>
    /// <returns>   Opaque data as a byte vector. </returns>
    public abstract byte[] DecodeOpaque( int length );

    /// <summary>
    /// Decodes (aka "deserializes") a XDR opaque value, which is represented by a vector of byte
    /// values, and starts at <paramref name="offset"/> with a length of <paramref name="length"/>.
    /// </summary>
    /// <remarks>
    /// Only the opaque value is decoded, so the caller has to know how long the opaque value will be. The
    /// decoded data is always padded to be a multiple of four (because that's what the sender does). <para>
    /// Derived classes must ensure that the proper semantic is maintained.</para>
    /// </remarks>
    /// <exception cref="XdrException">             Thrown when an XDR error condition occurs. </exception>
    /// <exception cref="System.IO.IOException">    Thrown when an I/O error condition occurs. </exception>
    /// <exception cref="IndexOutOfRangeException"> if the given <paramref name="opaque"/>
    ///                                             byte vector isn't large enough to receive the
    ///                                             result. </exception>
    /// <param name="opaque">   Byte vector which will receive the decoded opaque value. </param>
    /// <param name="offset">   Start offset in the byte vector. </param>
    /// <param name="length">   the number of bytes to decode. </param>
    public abstract void DecodeOpaque( byte[] opaque, int offset, int length );

    /// <summary>
    /// Decodes (aka "deserializes") a XDR opaque value, which is represented by a vector of byte
    /// values.
    /// </summary>
    /// <remarks>
    /// Only the opaque value is decoded, so the caller has to know how long the opaque value
    /// will be. The decoded data is always padded to be a multiple of four (because that's what the
    /// sender does).
    /// </remarks>
    /// <param name="opaque">   Byte vector which will receive the decoded opaque value. </param>
    ///
    /// <exception cref="XdrException">             Thrown when an XDR error condition occurs. </exception>
    /// <exception cref="System.IO.IOException">    Thrown when an I/O error condition occurs. </exception>
    public void DecodeOpaque( byte[] opaque )
    {
        this.DecodeOpaque( opaque, 0, opaque.Length );
    }

    /// <summary>
    /// Decodes (aka "deserializes") a XDR opaque value, which is represented by a vector of byte
    /// values.
    /// </summary>
    /// <remarks>
    /// The length of the opaque value to decode is pulled off of the XDR stream, so the
    /// caller does not need to know the exact length in advance. The decoded data is always padded
    /// to be a multiple of four (because that's what the sender does).
    /// </remarks>
    /// <returns>   The byte vector containing the decoded data. </returns>
    ///
    /// <exception cref="XdrException">             Thrown when an XDR error condition occurs. </exception>
    /// <exception cref="System.IO.IOException">    Thrown when an I/O error condition occurs. </exception>
    public byte[] DecodeDynamicOpaque()
    {
        int length = this.DecodeInt();
        byte[] opaque = new byte[length];
        if ( length != 0 )
        {
            this.DecodeOpaque( opaque );
        }
        return opaque;
    }

    /// <summary>
    /// Decodes (aka "deserializes") a vector of bytes, which is nothing more than a series of octets
    /// (or 8 bits wide bytes), each packed into its very own 4 bytes (XDR int).
    /// </summary>
    /// <remarks>
    /// Byte vectors are decoded together with a preceding length value. This way the receiver doesn't need to know
    /// the length of the vector in advance.
    /// </remarks>
    /// <returns>   The byte vector containing the decoded data. </returns>
    ///
    /// <exception cref="XdrException">             Thrown when an XDR error condition occurs. </exception>
    /// <exception cref="System.IO.IOException">    Thrown when an I/O error condition occurs. </exception>
    public byte[] DecodeByteVector()
    {
        int length = this.DecodeInt();
        return this.DecodeByteVector( length );
    }

    /// <summary>
    /// Decodes (aka "deserializes") a vector of bytes, which is nothing more than a series of octets
    /// (or 8 bits wide bytes), each packed into its very own 4 bytes (XDR int).
    /// </summary>
    /// <param name="length">   of vector to read. </param>
    /// <returns>   The byte vector containing the decoded data. </returns>
    ///
    /// <exception cref="XdrException">             Thrown when an XDR error condition occurs. </exception>
    /// <exception cref="System.IO.IOException">    Thrown when an I/O error condition occurs. </exception>
    public byte[] DecodeByteVector( int length )
    {
        if ( length > 0 )
        {
            byte[] bytes = new byte[length];
            for ( int i = 0; i < length; ++i )
            {
                bytes[i] = ( byte ) this.DecodeInt();
            }
            return bytes;
        }
        else
        {
            return new byte[0];
        }
    }

    /// <summary>   Decodes (aka "deserializes") a byte read from this XDR stream. </summary>
    /// <returns>   Decoded byte value. </returns>
    ///
    /// <exception cref="XdrException">             Thrown when an XDR error condition occurs. </exception>
    /// <exception cref="System.IO.IOException">    Thrown when an I/O error condition occurs. </exception>
    public byte DecodeByte()
    {
        return ( byte ) this.DecodeInt();
    }

    /// <summary>
    /// Decodes (aka "deserializes") a short (which is a 16 bit quantity)
    /// read from this XDR stream.
    /// </summary>
    /// <returns>   Decoded short value. </returns>
    ///
    /// <exception cref="XdrException">             Thrown when an XDR error condition occurs. </exception>
    /// <exception cref="System.IO.IOException">    Thrown when an I/O error condition occurs. </exception>
    public short DecodeShort()
    {
        return ( short ) this.DecodeInt();
    }

    /// <summary>
    /// Decodes (aka "deserializes") a long (which is called a "hyper" in XDR babble and is 64 bits
    /// wide) read from an XDR stream.
    /// </summary>
    /// <returns>   Decoded long value. </returns>
    ///
    /// <exception cref="XdrException">             Thrown when an XDR error condition occurs. </exception>
    /// <exception cref="System.IO.IOException">    Thrown when an I/O error condition occurs. </exception>
    public long DecodeLong()
    {

        // Similar to xdrEncodeLong: just read in two <see cref="int"/>'s in network order.  We
        // OR the int's together rather than adding them...

        return (( long ) this.DecodeInt() << 32) | (( long ) this.DecodeInt() & 0xffffffff);
    }

    /// <summary>
    /// Decodes (aka "deserializes") a float (which is a 32 bits wide floating point entity) read
    /// from an XDR stream.
    /// </summary>
    /// <returns>   Decoded float value. </returns>
    ///
    /// <exception cref="XdrException">             Thrown when an XDR error condition occurs. </exception>
    /// <exception cref="System.IO.IOException">    Thrown when an I/O error condition occurs. </exception>
    public float DecodeFloat()
    {
        return BitConverter.ToSingle( BitConverter.GetBytes( this.DecodeInt() ), 0 );
    }

    /// <summary>
    /// Decodes (aka "deserializes") a double (which is a 64 bits wide floating point entity) read
    /// from an XDR stream.
    /// </summary>
    /// <returns>   Decoded double value. </returns>
    ///
    /// <exception cref="XdrException">             Thrown when an XDR error condition occurs. </exception>
    /// <exception cref="System.IO.IOException">    Thrown when an I/O error condition occurs. </exception>
    public double DecodeDouble()
    {
        return BitConverter.Int64BitsToDouble( this.DecodeLong() );
    }

    /// <summary>   Decodes (aka "deserializes") a boolean read from an XDR stream. </summary>
    /// <returns>   Decoded boolean value. </returns>
    ///
    /// <exception cref="XdrException">             Thrown when an XDR error condition occurs. </exception>
    /// <exception cref="System.IO.IOException">    Thrown when an I/O error condition occurs. </exception>
    public bool DecodeBoolean()
    {
        return this.DecodeInt() != 0;
    }

    /// <summary>   Byte array to string. </summary>
    /// <param name="bytes">    The bytes. </param>
    /// <returns>   A string. </returns>
    internal string ByteArrayToStr( byte[] bytes )
    {
        ASCIIEncoding enc = new();
        return this.ByteArrayToStr( bytes, enc );
    }

    /// <summary>   Byte array to string. </summary>
    /// <param name="bytes">    The bytes. </param>
    /// <param name="enc">      The encode. </param>
    /// <returns>   A string. </returns>
    internal string ByteArrayToStr( byte[] bytes, Encoding enc )
    {
        return enc.GetString( bytes );
    }

    /// <summary>   Decodes (aka "deserializes") a string read from an XDR stream. </summary>
    /// <remarks>
    /// If a character encoding has been set for this stream, then this will be used for conversion.
    /// </remarks>
    /// <returns>   Decoded String value. </returns>
    ///
    /// <exception cref="XdrException">             Thrown when an XDR error condition occurs. </exception>
    /// <exception cref="System.IO.IOException">    Thrown when an I/O error condition occurs. </exception>
    public string DecodeString()
    {
        int length = this.DecodeInt();
        if ( length > 0 )
        {
            byte[] bytes = new byte[length];
            this.DecodeOpaque( bytes, 0, length );
            return (this.CharacterEncoding is not null)
                ? this.ByteArrayToStr( bytes, Encoding.GetEncoding( this.CharacterEncoding ) )
                : this.ByteArrayToStr( bytes );
        }
        else
        {
            return string.Empty;
        }
    }

    /// <summary>
    /// Decodes (aka "deserializes") a vector of short integers read from an XDR stream.
    /// </summary>
    /// <returns>   Decoded vector of short integers. </returns>
    ///
    /// <exception cref="XdrException">             Thrown when an XDR error condition occurs. </exception>
    /// <exception cref="System.IO.IOException">    Thrown when an I/O error condition occurs. </exception>
    public short[] DecodeShortVector()
    {
        int length = this.DecodeInt();
        return this.DecodeShortVector( length );
    }

    /// <summary>
    /// Decodes (aka "deserializes") a vector of short integers read from an XDR stream.
    /// </summary>
    /// <param name="length">   of vector to read. </param>
    /// <returns>   Decoded vector of short integers. </returns>
    ///
    /// <exception cref="XdrException">             Thrown when an XDR error condition occurs. </exception>
    /// <exception cref="System.IO.IOException">    Thrown when an I/O error condition occurs. </exception>
    public short[] DecodeShortVector( int length )
    {
        if ( length == 0 ) return Array.Empty<short>();
        short[] value = new short[length];
        for ( int i = 0; i < length; ++i )
        {
            value[i] = this.DecodeShort();
        }
        return value;
    }

    /// <summary>   Decodes (aka "deserializes") a vector of <see cref="int"/>'s read from an XDR stream. </summary>
    /// <returns>   Decoded int vector. </returns>
    ///
    /// <exception cref="XdrException">             Thrown when an XDR error condition occurs. </exception>
    /// <exception cref="System.IO.IOException">    Thrown when an I/O error condition occurs. </exception>
    public int[] DecodeIntVector()
    {
        int length = this.DecodeInt();
        return this.DecodeIntVector( length );
    }

    /// <summary>   Decodes (aka "deserializes") a vector of <see cref="int"/>'s read from an XDR stream. </summary>
    /// <param name="length">   of vector to read. </param>
    /// <returns>   Decoded int vector. </returns>
    ///
    /// <exception cref="XdrException">             Thrown when an XDR error condition occurs. </exception>
    /// <exception cref="System.IO.IOException">    Thrown when an I/O error condition occurs. </exception>
    public int[] DecodeIntVector( int length )
    {
        if ( length == 0 ) return Array.Empty<int>();
        int[] value = new int[length];
        for ( int i = 0; i < length; ++i )
        {
            value[i] = this.DecodeInt();
        }
        return value;
    }

    /// <summary>   Decodes (aka "deserializes") a vector of longs read from an XDR stream. </summary>
    /// <returns>   Decoded long vector. </returns>
    ///
    /// <exception cref="XdrException">             Thrown when an XDR error condition occurs. </exception>
    /// <exception cref="System.IO.IOException">    Thrown when an I/O error condition occurs. </exception>
    public long[] DecodeLongVector()
    {
        int length = this.DecodeInt();
        return this.DecodeLongVector( length );
    }

    /// <summary>   Decodes (aka "deserializes") a vector of longs read from an XDR stream. </summary>
    /// <param name="length">   of vector to read. </param>
    /// <returns>   Decoded long vector. </returns>
    ///
    /// <exception cref="XdrException">             Thrown when an XDR error condition occurs. </exception>
    /// <exception cref="System.IO.IOException">    Thrown when an I/O error condition occurs. </exception>
    public long[] DecodeLongVector( int length )
    {
        if ( length == 0 ) return Array.Empty<long>();
        long[] value = new long[length];
        for ( int i = 0; i < length; ++i )
        {
            value[i] = this.DecodeLong();
        }
        return value;
    }

    /// <summary>   Decodes (aka "deserializes") a vector of floats read from an XDR stream. </summary>
    /// <returns>   Decoded float vector. </returns>
    ///
    /// <exception cref="XdrException">             Thrown when an XDR error condition occurs. </exception>
    /// <exception cref="System.IO.IOException">    Thrown when an I/O error condition occurs. </exception>
    public float[] DecodeFloatVector()
    {
        int length = this.DecodeInt();
        return this.DecodeFloatVector( length );
    }

    /// <summary>   Decodes (aka "deserializes") a vector of floats read from an XDR stream. </summary>
    /// <param name="length">   of vector to read. </param>
    /// <returns>   Decoded float vector. </returns>
    ///
    /// <exception cref="XdrException">             Thrown when an XDR error condition occurs. </exception>
    /// <exception cref="System.IO.IOException">    Thrown when an I/O error condition occurs. </exception>
    public float[] DecodeFloatVector( int length )
    {
        if ( length == 0 ) return Array.Empty<float>();
        float[] value = new float[length];
        for ( int i = 0; i < length; ++i )
        {
            value[i] = this.DecodeFloat();
        }
        return value;
    }

    /// <summary>   Decodes (aka "deserializes") a vector of doubles read from an XDR stream. </summary>
    /// <returns>   Decoded double vector. </returns>
    ///
    /// <exception cref="XdrException">             Thrown when an XDR error condition occurs. </exception>
    /// <exception cref="System.IO.IOException">    Thrown when an I/O error condition occurs. </exception>
    public double[] DecodeDoubleVector()
    {
        int length = this.DecodeInt();
        return this.DecodeDoubleVector( length );
    }

    /// <summary>   Decodes (aka "deserializes") a vector of doubles read from an XDR stream. </summary>
    /// <param name="length">   of vector to read. </param>
    /// <returns>   Decoded double vector. </returns>
    ///
    /// <exception cref="XdrException">             Thrown when an XDR error condition occurs. </exception>
    /// <exception cref="System.IO.IOException">    Thrown when an I/O error condition occurs. </exception>
    public double[] DecodeDoubleVector( int length )
    {
        if ( length == 0 ) return Array.Empty<double>();
        double[] value = new double[length];
        for ( int i = 0; i < length; ++i )
        {
            value[i] = this.DecodeDouble();
        }
        return value;
    }

    /// <summary>   Decodes (aka "deserializes") a vector of booleans read from an XDR stream. </summary>
    /// <returns>   Decoded boolean vector. </returns>
    ///
    /// <exception cref="XdrException">             Thrown when an XDR error condition occurs. </exception>
    /// <exception cref="System.IO.IOException">    Thrown when an I/O error condition occurs. </exception>
    public bool[] DecodeBooleanVector()
    {
        int length = this.DecodeInt();
        return this.DecodeBooleanVector( length );
    }

    /// <summary>   Decodes (aka "deserializes") a vector of booleans read from an XDR stream. </summary>
    /// <param name="length">   of vector to read. </param>
    /// <returns>   Decoded boolean vector. </returns>
    ///
    /// <exception cref="XdrException">             Thrown when an XDR error condition occurs. </exception>
    /// <exception cref="System.IO.IOException">    Thrown when an I/O error condition occurs. </exception>
    public bool[] DecodeBooleanVector( int length )
    {
        if ( length == 0 ) return Array.Empty<bool>();
        bool[] value = new bool[length];
        for ( int i = 0; i < length; ++i )
        {
            value[i] = this.DecodeBoolean();
        }
        return value;
    }

    /// <summary>   Decodes (aka "deserializes") a vector of strings read from an XDR stream. </summary>
    /// <returns>   Decoded String vector. </returns>
    ///
    /// <exception cref="XdrException">             Thrown when an XDR error condition occurs. </exception>
    /// <exception cref="System.IO.IOException">    Thrown when an I/O error condition occurs. </exception>
    public string[] DecodeStringVector()
    {
        int length = this.DecodeInt();
        return this.DecodeStringVector( length );
    }

    /// <summary>   Decodes (aka "deserializes") a vector of strings read from an XDR stream. </summary>
    /// <param name="length">   of vector to read. </param>
    /// <returns>   Decoded String vector. </returns>
    ///
    /// <exception cref="XdrException">             Thrown when an XDR error condition occurs. </exception>
    /// <exception cref="System.IO.IOException">    Thrown when an I/O error condition occurs. </exception>
    public string[] DecodeStringVector( int length )
    {
        if ( length == 0 ) return Array.Empty<string>();
        string[] value = new string[length];
        for ( int i = 0; i < length; ++i )
        {
            value[i] = this.DecodeString();
        }
        return value;
    }

    /// <summary>
    /// Gets or sets the character encoding for deserializing strings. If <see langword="null"/>, the system's
    /// default encoding is to be used.
    /// </summary>
    /// <value> The character encoding. </value>
    public string CharacterEncoding { get; set; }

    #region " IDisposable Implementation "

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged
    /// resources.
    /// </summary>
    public void Dispose()
    {
        if ( this.IsDisposed ) { return; }
        try
        {
            // dispose of unmanaged resources
            this.Dispose( true );

            // suppress finalization
            GC.SuppressFinalize( this );
        }
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
        // set large fields to null
    }

    /// <summary>   Finalizer. </summary>
    ~XdrDecodingStreamBase()
    {
        if ( this.IsDisposed ) { return; }
        this.Dispose( false );
    }

    #endregion

}
