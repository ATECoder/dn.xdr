namespace cc.isr.XDR;

/// <summary>   Defines the abstract base class for all decoding XDR streams. </summary>
/// <remarks>
/// A decoding XDR stream returns data and primitive data types which it reads from 
/// a data source (for instance, network or memory buffer) in the platform-independent XDR format. <para>
/// 
/// Derived classes need to implement the <see cref="DecodeInt()"/>, <see cref="DecodeOpaque(int)"/> and
/// <see cref="DecodeOpaque(byte[], int, int)"/>. </para> <para>
/// 
/// Remote Tea authors: Harald Albrecht, Jay Walters. </para>
/// </remarks>
public abstract class XdrDecodingStreamBase : ICloseable
{

    #region " Construction and Cleanup "

    /// <summary>
    /// Closes this XDR stream and releases any system resources associated with this stream.
    /// </summary>
    /// <remarks>
    /// The general contract of <see cref="Close()"/> is that it closes and disposes of the decoding
    /// XDR stream. A closed XDR stream cannot perform decoding operations and cannot be reopened. <para>
    /// 
    /// The <see cref="XdrDecodingStreamBase.Close()"/> method of <see cref="XdrDecodingStreamBase"/>
    /// calls <see cref="Dispose()"/> and is not <see langword="virtual"/>.</para>
    /// </remarks>
    public void Close()
    {
        (( IDisposable ) this).Dispose();
    }

    #region " disposable implementation "

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged
    /// resources.
    /// </summary>
    /// <remarks>
    /// Takes account of and updates <see cref="IsDisposed"/>. Encloses <see cref="Dispose(bool)"/>
    /// within a try...finaly block. <para>
    ///
    /// Because this class is implementing <see cref="IDisposable"/> and is not sealed, then it
    /// should include the call to <see cref="GC.SuppressFinalize(object)"/> even if it does not
    /// include a user-defined finalizer. This is necessary to ensure proper semantics for derived
    /// types that add a user-defined finalizer but only override the protected <see cref="Dispose(bool)"/>
    /// method. </para> <para>
    /// 
    /// To this end, call <see cref="GC.SuppressFinalize(object)"/>, where <see langword="Object"/> = <see langword="this"/> in the <see langword="Finally"/> segment of
    /// the <see langword="try"/>...<see langword="catch"/> clause. </para><para>
    ///
    /// If releasing unmanaged code or freeing large objects then override <see cref="Object.Finalize()"/>. </para>
    /// </remarks>
    public void Dispose()
    {
        if ( this.IsDisposed ) { return; }
        try
        {
            // Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.

            this.Dispose( true );

        }
        catch { throw; }
        finally
        {
            // this is included because this class is not sealed.

            GC.SuppressFinalize( this );

            // mark things as disposed.

            this.IsDisposed = true;
        }
    }

    /// <summary>   Gets or sets a value indicating whether this object is disposed. </summary>
    /// <value> True if this object is disposed, false if not. </value>
    public bool IsDisposed { get; private set; }

    /// <summary>
    /// Releases unmanaged, large objects and (optionally) managed resources used by this class.
    /// </summary>
    /// <param name="disposing">    True to release large objects and managed and unmanaged resources;
    ///                             false to release only unmanaged resources and large objects. </param>
    protected virtual void Dispose( bool disposing )
    {
        if ( disposing )
        {
            // dispose managed state (managed objects)
        }

        // free unmanaged resources and override finalizer

        // set large fields to null
    }

    #endregion

    #endregion

    #region " defaults "

    /// <summary>   Gets or sets the default size of the buffer. </summary>
    /// <value> The buffer size default. </value>
    public static int BufferSizeDefault { get; set; } = 8192;

    /// <summary>   Gets or sets the default minimum size of the buffer. </summary>
    /// <value> The minimum buffer size default. </value>
    public static int MinBufferSizeDefault { get; set; } = 1024;

    /// <summary>   Gets or sets the default character encoding. </summary>
    /// <summary>   Gets or sets the default character encoding. </summary>
    /// <remarks>
    /// The default encoding for VXI-11 is <see cref="Encoding.ASCII"/>, which is a subset of <see cref="Encoding.UTF8"/>
    /// </remarks>
    /// <value> The default encoding. </value>
    public static Encoding EncodingDefault { get; set; } = Encoding.UTF8;

    #endregion

    #region " members "

    /// <summary>
    /// Gets the remote <see cref="IPEndPoint"/> with which the socket is communicating. 
    /// </summary>
    /// <remarks>
    /// With UDP decoding, this value is valid only after <see cref="BeginDecoding()"/>, otherwise it might return stale information.
    /// </remarks>
    /// <value> The remote endpoint. </value>
    public virtual IPEndPoint RemoteEndPoint => new( IPAddress.Any, 0 );

    /// <summary>
    /// Gets or sets the character encoding for deserializing strings. 
    /// </summary>
    /// <value> The character encoding. </value>
    public Encoding CharacterEncoding { get; set; } = XdrDecodingStreamBase.EncodingDefault;

    #endregion

    #region " actions "

    /// <summary>   Initiates decoding of the next XDR record. </summary>
    /// <remarks>
    /// This typically involves filling the internal buffer with the next datagram from the network,
    /// or reading the next chunk of data from a stream-oriented connection. In case of memory-based
    /// communication this might involve waiting for some other process to fill the buffer and signal
    /// availability of new XDR data.
    /// </remarks>
    public abstract void BeginDecoding();

    /// <summary>   End decoding of the current XDR record. </summary>
    /// <remarks>
    /// The general contract of <see cref="EndDecoding"/> is that calling it is
    /// an indication that the current record is no more interesting to the caller and any allocated 
    /// data for this record can be freed. <para>
    /// 
    /// The <see cref="XdrDecodingStreamBase.EndDecoding"/> method of <see cref="XdrDecodingStreamBase"/>
    /// does nothing. </para>
    /// </remarks>
    public virtual void EndDecoding()
    { }
    #endregion

    #region " decoding "

    /// <summary>   Decodes (aka "deserializes") a "XDR int" value received from an XDR stream. </summary>
    /// <remarks>
    /// An XDR int encapsulate a 32 bits <see cref="int"/>.
    /// This method is one of the basic methods all other methods can rely on.
    /// Because it's so basic, derived classes have to implement it.
    /// </remarks>
    /// <exception cref="XdrException">  Thrown when an XDR error condition occurs. </exception>
    /// <returns>   The decoded int value. </returns>
    public abstract int DecodeInt();

    /// <summary>   Decodes (aka "deserializes") a XDR <see cref="uint"/> value received from an XDR stream. </summary>
    /// <remarks>
    /// An XDR <see cref="uint"/> encapsulate a 32 bits <see cref="uint"/> in 4 bytes.
    /// </remarks>
    /// <exception cref="XdrException">  Thrown when an XDR error condition occurs. </exception>
    /// <returns>   The decoded <see cref="uint"/> value. </returns>
    public abstract uint DecodeUInt();

    /// <summary>
    /// Decodes (aka "deserializes") XDR opaque data, which consists of an array of <see cref="byte"/>s
    /// of length that is a multiple of 4.
    /// </summary>
    /// <remarks>
    /// Allocates sufficient <see cref="byte"/>s to copy and return a subset of the internal <see cref="Buffer"/> <para>
    /// 
    /// Because the length of the opaque value is given, we don't need to retrieve it from the XDR
    /// stream. This is different from <see cref="XdrDecodingStreamBase.DecodeDynamicOpaque()"/>
    /// where first the length of the opaque data is retrieved from the XDR stream. </para>
    /// </remarks>
    /// <exception cref="XdrException"> Thrown when an XDR error condition occurs. </exception>
    /// <param name="length">   Length of opaque data to decode. </param>
    /// <returns>   Opaque data as a <see cref="byte"/> vector. </returns>
    public abstract byte[] DecodeOpaque( int length );

    /// <summary>
    /// Decodes (aka "deserializes") XDR opaque data, which consists of an array of <see cref="byte"/>s of length
    /// that is a multiple of 4 and starts at <paramref name="offset"/> with a length of
    /// <paramref name="length"/>.
    /// </summary>
    /// <remarks>
    /// Allocates sufficient <see cref="byte"/>s to copy and return a subset of the internal <see cref="Buffer"/> <para>
    /// 
    /// Only the opaque value is decoded, so the caller has to know how long the opaque value will
    /// be. The decoded data is always padded to be a multiple of four (because that's what the
    /// sender does). </para>
    /// </remarks>
    /// <exception cref="XdrException"> Thrown when an XDR error condition occurs. </exception>
    /// <param name="opaque">   <see cref="byte"/> vector which will receive the decoded opaque value. </param>
    /// <param name="offset">   Start offset in the <see cref="byte"/> vector. </param>
    /// <param name="length">   the number of <see cref="byte"/>s to decode. </param>
    public abstract void DecodeOpaque( byte[] opaque, int offset, int length );

    /// <summary>
    /// Decodes (aka "deserializes") XDR opaque data, which consists of an array of <see cref="byte"/>s
    /// of length that is a multiple of 4.
    /// </summary>
    /// <remarks>
    /// Allocates sufficient <see cref="byte"/>s to copy and return a subset of the internal <see cref="Buffer"/> <para>
    /// 
    /// Only the opaque value is decoded, so the caller has to know how long the opaque value
    /// will be. The decoded data is always padded to be a multiple of four (because that's what the
    /// sender does). </para>
    /// </remarks>
    /// <exception cref="XdrException">  Thrown when an XDR error condition occurs. </exception>
    /// <param name="opaque">   <see cref="byte"/> vector which will receive the decoded opaque value. </param>
    public void DecodeOpaque( byte[] opaque )
    {
        this.DecodeOpaque( opaque, 0, opaque.Length );
    }

    /// <summary>
    /// Decodes (aka "deserializes") XDR variable length opaque data, which is represented by a
    /// vector of <see cref="byte"/> values of length that is a multiple of 4.
    /// </summary>
    /// <remarks>
    /// The length of the opaque value to decode is pulled off of the XDR stream, so the caller does
    /// not need to know the exact length in advance. The decoded data is always padded to be a
    /// multiple of four (because that's what the sender does).
    /// </remarks>
    /// <exception cref="XdrException">  Thrown when an XDR error condition occurs. </exception>
    /// <returns>   The <see cref="byte"/> vector containing the decoded data. </returns>
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
    /// Decodes (aka "deserializes") XDR variable length opaque data, which is represented by a
    /// vector of <see cref="char"/> values of length that is a multiple of 4.
    /// </summary>
    /// <remarks>
    /// The length of the opaque value to decode is pulled off of the XDR stream, so the caller does
    /// not need to know the exact length in advance. The decoded data is always padded to be a
    /// multiple of four (because that's what the sender does).
    /// </remarks>
    /// <exception cref="XdrException">  Thrown when an XDR error condition occurs. </exception>
    /// <returns>   The <see cref="char"/> vector containing the decoded data. </returns>
    public char[] DecodeDynamicOpaqueChar()
    {
        int length = this.DecodeInt();
        byte[] opaque = new byte[length];
        if ( length != 0 )
        {
            this.DecodeOpaque( opaque );
        }
        return this.CharacterEncoding.GetChars( opaque );
    }

    /// <summary>
    /// Decodes (aka "deserializes") a vector of <see cref="byte"/>s, which is nothing more than a series of octets
    /// (or 8 bits wide <see cref="byte"/>s), each packed into its very own 4 <see cref="byte"/>s (XDR int).
    /// </summary>
    /// <remarks>
    /// <see cref="byte"/> vectors are decoded together with a preceding length value. This way the receiver doesn't need to know
    /// the length of the vector in advance.
    /// </remarks>
    /// <exception cref="XdrException">  Thrown when an XDR error condition occurs. </exception>
    /// <returns>   The <see cref="byte"/> vector containing the decoded data. </returns>
    public byte[] DecodeByteVector()
    {
        int length = this.DecodeInt();
        return this.DecodeByteVector( length );
    }

    /// <summary>
    /// Decodes (aka "deserializes") a vector of <see cref="byte"/>s, which is nothing more than a series of octets
    /// (or 8 bits wide <see cref="byte"/>s), each packed into its very own 4 <see cref="byte"/>s (XDR int).
    /// </summary>
    /// <exception cref="XdrException">  Thrown when an XDR error condition occurs. </exception>
    /// <param name="length">   of vector to read. </param>
    /// <returns>   The <see cref="byte"/> vector containing the decoded data. </returns>
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

    /// <summary>   Decodes (aka "deserializes") a <see cref="byte"/> read from this XDR stream. </summary>
    /// <returns>   Decoded <see cref="byte"/> value. </returns>
    /// <exception cref="XdrException">  Thrown when an XDR error condition occurs. </exception>
    public byte DecodeByte()
    {
        return ( byte ) this.DecodeInt();
    }

    /// <summary>   Decodes (aka "deserializes") a <see cref="char"/> read from this XDR stream. </summary>
    /// <returns>   Decoded <see cref="char"/> value. </returns>
    /// <exception cref="XdrException">  Thrown when an XDR error condition occurs. </exception>
    public char DecodeChar()
    {
        return ( char ) this.DecodeByte();
    }

    /// <summary>
    /// Decodes (aka "deserializes") a <see cref="short"/> (which is a 16 bit quantity)
    /// read from this XDR stream.
    /// </summary>
    /// <returns>   Decoded <see cref="short"/> value. </returns>
    /// <exception cref="XdrException">  Thrown when an XDR error condition occurs. </exception>
    public short DecodeShort()
    {
        return ( short ) this.DecodeInt();
    }

    /// <summary>
    /// Decodes (aka "deserializes") a <see cref="long"/> (which is called a "hyper" in XDR babble and is 64 bits
    /// wide) read from an XDR stream.
    /// </summary>
    /// <returns>   Decoded <see cref="long"/> value. </returns>
    /// <exception cref="XdrException">  Thrown when an XDR error condition occurs. </exception>
    public long DecodeLong()
    {

        // Similar to xdrEncodeLong: just read in two <see cref="int"/>'s in network order.  We
        // OR the int's together rather than adding them...

        return (( long ) this.DecodeInt() << 32) | (( long ) this.DecodeInt() & 0xffffffff);
    }

    /// <summary>
    /// Decodes (aka "deserializes") a <see cref="float"/> (which is a 32 bits wide floating point entity) read
    /// from an XDR stream.
    /// </summary>
    /// <returns>   Decoded <see cref="float"/> value. </returns>
    /// <exception cref="XdrException">  Thrown when an XDR error condition occurs. </exception>
    public float DecodeFloat()
    {
        return BitConverter.ToSingle( BitConverter.GetBytes( this.DecodeInt() ), 0 );
    }

    /// <summary>
    /// Decodes (aka "deserializes") a <see cref="double"/> (which is a 64 bits wide floating point entity) read
    /// from an XDR stream.
    /// </summary>
    /// <returns>   Decoded <see cref="double"/> value. </returns>
    /// <exception cref="XdrException">  Thrown when an XDR error condition occurs. </exception>
    public double DecodeDouble()
    {
        return BitConverter.Int64BitsToDouble( this.DecodeLong() );
    }

    /// <summary>   Decodes (aka "deserializes") a boolean read from an XDR stream. </summary>
    /// <returns>   Decoded boolean value. </returns>
    /// <exception cref="XdrException">  Thrown when an XDR error condition occurs. </exception>
    public bool DecodeBoolean()
    {
        return this.DecodeInt() != 0;
    }

    /// <summary>   Decodes (aka "deserializes") a <see cref="string"/> read from an XDR stream. </summary>
    /// <remarks>
    /// If a character encoding has been set for this stream, then this will be used for conversion.
    /// </remarks>
    /// <returns>   Decoded <see cref="string"/> value. </returns>
    /// <exception cref="XdrException">  Thrown when an XDR error condition occurs. </exception>
    public string DecodeString()
    {
        int length = this.DecodeInt();
        if ( length > 0 )
        {
            byte[] bytes = new byte[length];
            this.DecodeOpaque( bytes, 0, length );
            return this.CharacterEncoding.GetString( bytes );
        }
        else
        {
            return string.Empty;
        }
    }

    /// <summary>
    /// Decodes (aka "deserializes") a vector of <see cref="short"/> integers read from an XDR stream.
    /// </summary>
    /// <returns>   Decoded vector of <see cref="short"/> integers. </returns>
    /// <exception cref="XdrException">  Thrown when an XDR error condition occurs. </exception>
    public short[] DecodeShortVector()
    {
        int length = this.DecodeInt();
        return this.DecodeShortVector( length );
    }

    /// <summary>
    /// Decodes (aka "deserializes") a vector of <see cref="short"/> integers read from an XDR stream.
    /// </summary>
    /// <exception cref="XdrException">  Thrown when an XDR error condition occurs. </exception>
    /// <param name="length">   of vector to read. </param>
    /// <returns>   Decoded vector of <see cref="short"/> integers. </returns>
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
    /// <returns>   Decoded <see cref="int"/> vector. </returns>
    /// <exception cref="XdrException">  Thrown when an XDR error condition occurs. </exception>
    public int[] DecodeIntVector()
    {
        int length = this.DecodeInt();
        return this.DecodeIntVector( length );
    }

    /// <summary>   Decodes (aka "deserializes") a vector of <see cref="int"/>'s read from an XDR stream. </summary>
    /// <param name="length">   of vector to read. </param>
    /// <returns>   Decoded <see cref="int"/> vector. </returns>
    /// <exception cref="XdrException">  Thrown when an XDR error condition occurs. </exception>
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

    /// <summary>   Decodes (aka "deserializes") a vector of <see cref="long"/>s read from an XDR stream. </summary>
    /// <returns>   Decoded <see cref="long"/> vector. </returns>
    /// <exception cref="XdrException">  Thrown when an XDR error condition occurs. </exception>
    public long[] DecodeLongVector()
    {
        int length = this.DecodeInt();
        return this.DecodeLongVector( length );
    }

    /// <summary>   Decodes (aka "deserializes") a vector of <see cref="long"/>s read from an XDR stream. </summary>
    /// <param name="length">   of vector to read. </param>
    /// <returns>   Decoded <see cref="long"/> vector. </returns>
    /// <exception cref="XdrException">  Thrown when an XDR error condition occurs. </exception>
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

    /// <summary>   Decodes (aka "deserializes") a vector of <see cref="float"/>s read from an XDR stream. </summary>
    /// <returns>   Decoded <see cref="float"/> vector. </returns>
    /// <exception cref="XdrException">  Thrown when an XDR error condition occurs. </exception>
    public float[] DecodeFloatVector()
    {
        int length = this.DecodeInt();
        return this.DecodeFloatVector( length );
    }

    /// <summary>   Decodes (aka "deserializes") a vector of <see cref="float"/>s read from an XDR stream. </summary>
    /// <param name="length">   of vector to read. </param>
    /// <returns>   Decoded <see cref="float"/> vector. </returns>
    /// <exception cref="XdrException">  Thrown when an XDR error condition occurs. </exception>
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

    /// <summary>   Decodes (aka "deserializes") a vector of <see cref="double"/>s read from an XDR stream. </summary>
    /// <returns>   Decoded <see cref="double"/> vector. </returns>
    /// <exception cref="XdrException">  Thrown when an XDR error condition occurs. </exception>
    public double[] DecodeDoubleVector()
    {
        int length = this.DecodeInt();
        return this.DecodeDoubleVector( length );
    }

    /// <summary>   Decodes (aka "deserializes") a vector of <see cref="double"/>s read from an XDR stream. </summary>
    /// <param name="length">   of vector to read. </param>
    /// <returns>   Decoded <see cref="double"/> vector. </returns>
    /// <exception cref="XdrException">  Thrown when an XDR error condition occurs. </exception>
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

    /// <summary>   Decodes (aka "deserializes") a vector of <see cref="bool"/>s read from an XDR stream. </summary>
    /// <returns>   Decoded <see cref="bool"/> vector. </returns>
    /// <exception cref="XdrException">  Thrown when an XDR error condition occurs. </exception>
    public bool[] DecodeBooleanVector()
    {
        int length = this.DecodeInt();
        return this.DecodeBooleanVector( length );
    }

    /// <summary>   Decodes (aka "deserializes") a vector of <see cref="bool"/>s read from an XDR stream. </summary>
    /// <param name="length">   of vector to read. </param>
    /// <returns>   Decoded <see cref="bool"/> vector. </returns>
    /// <exception cref="XdrException">  Thrown when an XDR error condition occurs. </exception>
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

    /// <summary>   Decodes (aka "deserializes") a vector of <see cref="bool"/>s read from an XDR stream. </summary>
    /// <returns>   Decoded <see cref="bool"/> vector. </returns>
    /// <exception cref="XdrException">  Thrown when an XDR error condition occurs. </exception>
    public string[] DecodeStringVector()
    {
        int length = this.DecodeInt();
        return this.DecodeStringVector( length );
    }

    /// <summary>   Decodes (aka "deserializes") a vector of <see cref="bool"/>s read from an XDR stream. </summary>
    /// <param name="length">   of vector to read. </param>
    /// <returns>   Decoded <see cref="bool"/> vector. </returns>
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

    /// <summary>   Decodes the IP address. </summary>
    /// <returns>   The IPAddress. </returns>
    public IPAddress DecodeIPAddress()
    {
        byte[] bytes = new byte[4];
        this.DecodeOpaque( bytes );
        // flip little-endian to big-endian(network order)
        if ( BitConverter.IsLittleEndian )
        {
            Array.Reverse( bytes );
        }
        return new IPAddress( bytes );
    }

    #endregion

}
