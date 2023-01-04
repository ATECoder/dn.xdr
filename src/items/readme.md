# About

[ISR's XDR] is a C# implementation of the [XDR: External Data Representation Standard (May 2006)] as implemented in [Java ONC RPC] implementation called Remote Tea.

## History

[ISR's XDR] and [ISR's ONC RPC] were forked from [GB1.RemoteTea.Net], which was forked from [Wes Day's GitHub repository], which is a fork of 
[Jay Walter's SourceForge repository], which is a port of [Java ONC RPC].

## Standards

* [XDR: External Data Representation Standard (May 2006)]

## How to Use

[ISR's XDR] and [ISR's ONC RPC] MS Test projects include examples for using [ISR's XDR].

## Departures from [GB1.RemoteTea.Net]

* The base namespace was changed from org.acplt to cc.isr;
* The ONC/RPC namespace was changed from org.acplt.oncrpc to cc.isr.XDR;
* Interface names are prefixed with 'I';
* Base class names are suffixes with Base;
* the xdrAble interface was renamed to IXdrCodec;
* The xdr prefixes were removed from the codec methods;
* Uppercase constant names were converted to Pascal casing while retaining the original constant names in the code documentation. 

## Feedback

[ISR's XDR] is released as open source under the MIT license.
Bug reports and contributions are welcome at the [ISR's XDR] repository.

[ISR's XDR]: https://github.com/ATECoder/dn.xdr
[ISR's ONC RPC]: https://github.com/ATECoder/dn.onc.rpc
[XDR: External Data Representation Standard (May 2006)]: http://tools.ietf.org/html/rfc4506

[Jay Walter's SourceForge repository]: https://sourceforge.net/p/remoteteanet
[Wes Day's GitHub repository]: https://github.com/wespday/RemoteTea.Net
[GB1.RemoteTea.Net]: https://github.com/galenbancroft/RemoteTea.Net
[org.acplt.oncrpc package]: https://people.eecs.berkeley.edu/~jonah/javadoc/org/acplt/oncrpc/package-summary.html
[Java ONC RPC]: https://github.com/remotetea/remotetea/tree/master/src/tests/org/acplt/oncrpc

