namespace cc.isr.XDR.MSTest.Codecs
{
    internal class FooBazCodec : FooCodecBase
    {

        public FooBazCodec()
        {
        }

        public FooBazCodec( float value )
        {
            this.Noah = value;
        }

        /// <summary>   Gets or sets the encapsulated value. </summary>
        /// <value> The encapsulated value. </value>
        public float Noah { get; private set; }

        public override void Decode( XdrDecodingStreamBase xdr )
        {
            // decode your members here...
            // but *NEVER* decode the discriminant value
            this.Noah = xdr.DecodeFloat();
        }

        public override void Encode( XdrEncodingStreamBase xdr )
        {
            // encode your members here...
            // don't forget to encode the discriminant value
            xdr.EncodeInt( FooCodecBase.FooBazClass );
            xdr.EncodeFloat( this.Noah );
        }
    }
}
