## About
This bridge emulates an IRC leaf server. Users joined to the leaf are secretly discord users, however from the perspective of the hub, they are just normal users. 
## Setup
### Requirements
* UnrealIRCD - This product not tested on other animals. 
* You will need oper / root priviledges on the server this service is installed on. The service operates a bit like a server link between a hub and a leaf.  
* Windows or Mono. .Net Core might work, but 🤷
* This currently Relies on WinForms. During the early stages of development, I'm initializing the libraries from here, rather than from a service or console application, at least until I solidify how I want the library to work. Think of this as a scaffolding that will be removed once the building is complete. 

### Build the Project
* Open the Bridge.sln file in Visual Studio.
* Build the project.
* After building, Bridge.exe may be moved to wherever you like. Copy this and all DLL files from build directory into their own home (or just leave them.. I don't really care) 

### Get your Discord Token
* There are dozens of guides out there on how to do this. Use google. 

### Configure a ULine for the Bridge Service. 
* In my example, I am using port 7001. and a Password of GoldenRetriever. 
* Make sure that wherever BridgeMock_May2019.exe is run from, that it can reach the port/ip. use nmap to verify. 

**unrealircd.conf**
```
listen { 
	ip *;
	port 7001;
};
... 
link bridgeserv.teamofprogrammers.com
{
        incoming {
                mask *;
		              port 7001;
	       };
        password "GoldenRetriever";
        class servers;
};
...
ulines { 
	bridgeserv.teamofprogrammers.com;
};

```


### Setup the XML Configuration file
*  Place config.xml in the same directory as Bridge.exe.
* Channel Mappings:
  * IRC is the channel name on the IRC server
  * Discord is the channel UID on the Discord server.
    * To get the ID from discord, you can right click the channel and click Copy ID. 
* To get the Guild Id, you can right click your server icon in discord, and click Copy ID.

**Config.Xml**
```XML
<?xml version="1.0" encoding="utf-8" ?>    
<BridgeConfig>
  <IRCServer>
    <UplinkHost>127.0.0.1</UplinkHost>
    <UplinkPort>7001</UplinkPort>
    <UplinkPassword>ulinepassword</UplinkPassword>
    <ServerIdentifier>00E</ServerIdentifier>
    <ServerName>subdomain.domain.com</ServerName>
    <ServerDescription>Discord Bridge</ServerDescription>
    <NicknameSuffix>♥</NicknameSuffix>
    <MaxMessageSize>350</MaxMessageSize>
    <SqueezeWhiteSpace>true</SqueezeWhiteSpace>
  </IRCServer>
  <DiscordServer>
    <Token>VERYSECRETTOKEN Get from https://discordapp.com/developers/applications/ </Token>
    <GuildId>000000000000000000</GuildId>
    <IgnoredRoles></IgnoredRoles>
    <OnlineStatuses>AFK,Idle,Online</OnlineStatuses>
    <ChannelMapping>
      <Channel>
        <IRC>#channel1</IRC>
        <Discord>000000000000000000</Discord>
      </Channel>
      <Channel>
        <IRC>#channel2</IRC>
        <Discord>000000000000000000</Discord>
      </Channel>
      <Channel>
        <IRC>#channel3</IRC>
        <Discord>000000000000000000</Discord>
      </Channel>      
    </ChannelMapping>
  </DiscordServer>
</BridgeConfig>
```
## Contributing
If you want to jump in, check out items in our [TODO] list, check for any open issues, or just run the code and provide us with feedback and bug reports. Help us keep IRC alive. 
