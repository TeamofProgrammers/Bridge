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
  
