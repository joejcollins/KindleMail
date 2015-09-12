# Low Vision Web Mail Client #



## Set Up Remote Client for Development ##

External/Remote device attach to IIS Express

* Give your development platform a static IP.  For convenience you might want to do this using address reservation on the router.

* Add binding to IISExpress Config (applicationhost.config)

<binding protocol="http" bindingInformation="*:56328:192.168.0.3" />
 
* On the command line tell http.sys that it’s ok to talk to this url

netsh http add urlacl url=http://192.168.0.3:56328/ user=everyone
	This just tells http.sys that it’s ok to talk to this url.

* On the command line add a rule in the Windows Firewall, allowing incoming connections

netsh advfirewall firewall add rule name="IISExpressWeb" dir=in protocol=tcp localport=56328 profile=private remoteip=localsubnet action=allow
	This 