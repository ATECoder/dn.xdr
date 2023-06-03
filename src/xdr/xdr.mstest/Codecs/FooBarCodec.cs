namespace cc.isr.XDR.MSTest.Codecs;

/// <summary>   A foo bar codec. </summary>
internal class FooBarCodec : FooCodecBase
{

    /// <summary>   Default constructor. </summary>
    public FooBarCodec()
    {
    }

    /// <summary>   Constructor. </summary>
    /// <param name="value">    The value. </param>
    public FooBarCodec( float value )
    {
        this.Noah = value;
    }

    /// <summary>   Gets or sets the encapsulated value. </summary>
    /// <value> The encapsulated value. </value>
    public float Noah { get; private set; }

    /// <summary>
    /// Decodes -- that is: deserializes -- a XDR float from an XDR stream in compliance to RFC 1832.
    /// </summary>
    /// <param name="decoder">  XDR stream from which decoded information is retrieved. </param>
    public override void Decode( XdrDecodingStreamBase decoder )
    {
        // decode your members here...
        // but *NEVER* decode the discriminant value
        this.Noah = decoder.DecodeFloat();
    }

    /// <summary>
    /// Encodes -- that is: serializes -- a XDR float into an XDR stream in compliance to RFC 1832.
    /// </summary>
    /// <param name="encoder">  XDR stream to which information is sent for encoding. </param>
    public override void Encode( XdrEncodingStreamBase encoder )
    {
        // encode your members here...
        // don't forget to encode the discriminant value
        encoder.EncodeInt( FooCodecBase.FooBarClass );
        encoder.EncodeFloat( this.Noah );
    }
}
