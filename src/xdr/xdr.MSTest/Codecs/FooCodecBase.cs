namespace cc.isr.XDR.MSTest.Codecs
{
    /// <summary>   A foo codec base. </summary>
    /// <remarks>
    /// You can also take advantage of polymorphism when translating discriminated unions into a
    /// class hierarchy. You should then write an abstract base class with only one static member
    /// function capable of constructing the appropriate instance from the XDR stream. The tricky
    /// part about this is getting the discriminant value into the stream or out of it without having
    /// to duplicate the decoding code into the constructor. With the skeleton below you should be
    /// able to do this easily, but you should remember to <b>never recycle an object reference</b>
    /// and <b>never deserialize the state of the object a second time</b> from a XDR stream!
    /// </remarks>
    internal abstract class FooCodecBase : IXdrCodec
    {

        // discriminant values
        public const int FooBarClass = 1;
        public const int FooBazClass = 2;

        /// <summary>   Constructs a bar, baz,... class from XDR stream. </summary>
        /// <remarks>   2023-01-02. </remarks>
        /// <param name="decoder">  The XDR decoding stream. </param>
        /// <returns>   A FooCodecBase? </returns>
        public static FooCodecBase? XdrNew( XdrDecodingStreamBase decoder )
        {
            FooCodecBase? obj = null;
            switch ( decoder.DecodeInt() )
            {
                case FooCodecBase.FooBarClass:
                    obj = new FooBarCodec();
                    break;
                case FooCodecBase.FooBazClass:
                    obj = new FooBazCodec();
                    break;
            }
            obj?.Decode( decoder );
            return obj;
        }

        /// <summary>
        /// Decodes -- that is: deserializes -- an object from an XDR stream in compliance to RFC 1832.
        /// </summary>
        /// <remarks>   2023-01-05. </remarks>
        /// <exception cref="T:cc.isr.XDR.XdrException">    Thrown when an XDR error condition occurs. </exception>
        /// <exception cref="T:System.IO.IOException">      Thrown when an I/O error condition occurs. </exception>
        /// <param name="decoder">  The decoder. </param>
        public abstract void Decode( XdrDecodingStreamBase decoder );

        /// <summary>
        /// Encodes -- that is: serializes -- an object into an XDR stream in compliance to RFC 1832.
        /// </summary>
        /// <param name="encoder">  XDR stream to which information is sent for encoding. </param>
        public abstract void Encode( XdrEncodingStreamBase encoder );
    }
}

