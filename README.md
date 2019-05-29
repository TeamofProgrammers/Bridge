# Bridge
## ToDo:
* Handle Usernames With Spaces
* General Input / Output validation. 
  * Discord can send a new line with alt+enter. How does irc handle this? 
* What happens when User gets kicked from IRC, but still are in discord?
* What happens when user changes nick on discord
  * Currently doesn't matter, as I am looking at their username and ignoring thier chosen nickname.
* What happens when a user is @ referenced in discord? Do we translate that on irc?
* What happens when a user is highlighted in irc? do we highlight them in discord?
* What happens with nickname collisions?
  * Need to implement a prefix/ suffix system. 
* Should we ignore other bots?
* What happens when a discord user receives a private message on irc?
  * We could send them a PM from the bot, notifying them they have a message on irc...
  * Or we can inform the irc user sending the message that this is not supported.
  * It would be possible to have the bot relay the message from that user, and then offer a text based ui for switching between private message targets
* Is there a message for netsplits, or do clients just handle this automatically when they see a large quantity of joins/parts? 
  * I would like for it to show netsplit when the service is killed/rejoined. 
* Rename from BridgeMock_May2019 to something less... boring. 
* Allow for channel names in config.xml, convert them to the uint variant in the ReadConfig function. 
* Get the config.xml parsing logic out of Discord.cs and BridgeService.cs, isolate this entirely in the entry point of the program. These libraries shouldn't be reading from a config file, as it is outside of their scope. 
* Add SSL/TLS to the connection.
* Allow for Hostnames to be used in the ServerHost section. 

## Setup
### Requirements
* UnrealIRCD - This product not tested on other animals. 
* It should go without saying, but you will need oper / root priviledges on the server this service is installed on. The service operates a bit like a server link between a 
* Windows or Mono. .Net Core might work, but 🤷
* This currently Relies on WinForms. During the early stages of development, I'm initializing the libraries from here, rather than from a service or console application, at least until I solidify how I want the library to work. Think of this as a scaffolding that will be removed once the building is complete. 

### Build the Project
* Open the BridgeMock_May2019.sln file in Visual Studio.
* Build the project.
* After building, BridgeMock_May2019.exe may be moved to wherever you like. Copy this and all DLL files from build directory into their own home (or just leave them.. I don't really care) 

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
*  Place config.xml in the same directory as BridgeMock_May2019.exe.
* Channel Mappings:
  * IRC is the channel name on the IRC server
  * Discord is the channel UID on the Discord server.
    * To get the ID from discord, you can right click the channel and click Copy ID. 
* To get the Guild Id, you can right click your server icon in discord, and click Copy ID.
** Config.Xml **
```XML
<?xml version="1.0" encoding="utf-8" ?>
<BridgeConfig>
	<IRCServer>
		<ServerHost>192.168.1.8</ServerHost>
		<ServerPort>7001</ServerPort>
		<ServerPassword>GoldenRetriever</ServerPassword>
		<ServerIdentifier>00C</ServerIdentifier>
	</IRCServer>
	<DiscordServer>
		<Token>Discord Token Goes Here</Token>
		<GuildID>294590221843038209</GuildID>
		<ChannelMapping>
			<Channel>
				<IRC>#Top</IRC>
				<Discord>294593199710797824</Discord>
			</Channel>
			<Channel>
				<IRC>#politics</IRC>
				<Discord>299967713948860417</Discord>
			</Channel>
			<Channel>
				<IRC>#Bot-Test</IRC>
				<Discord>388908063014387714</Discord>
			</Channel>
			<Channel>
				<IRC>#PLN</IRC>
				<Discord>400130756812472320</Discord>
			</Channel>
		</ChannelMapping>
	</DiscordServer>
</BridgeConfig>
```
