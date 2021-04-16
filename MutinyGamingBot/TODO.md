# To-Do

## Common

### Configuration.cs

- Quit application after creating template config
- Better check on token?

## Database

### MutinyBotDbContext.cs

- Find a less lazy and much more robust way of implementing migrations for production settings
	- Kinda done?

## Entities

### GuildEntity.cs

- Add bool value for enabling birthday messages
- Remove GuildName or have it used for console messages
- Moderation log channel

### MemberEntity.cs

- Add DateTime property for last message
- Add int TimesMuted propery
- Add Nationality flag emoji field?

### UserEntity.cs

- Add DateTime property for birthdays
- Add Nationality property

## Modules

### AdminModule.cs

- Implement AddMuteRole command to mute a user for a period of time and increment user TimesMuted 
- Implement GetInactiveUsers command using MemberEntity last message property

### CreatorModule.cs

### MemeModule.cs

### MutinyGuildModule.cs

- Implement traingings/instructor command
- Implement mascot/cat command
- Implement arma mod command

### RoleModule.cs

- Simplify code for paginated embed

### UserModule.cs

- Implement salary field
	- "Take it up with my superior" - Kelsen
	- "I am the community -" - Prius

## Services

### EventService.cs

- Fix problem regarding OnGuildAvailable() and OnGuildJoined() taking more than 1 second from UpdateGuild() for guilds with large member counts
	- Change UpdateGuild() to use transaction instead?

### GuildService.cs

- Implement RemoveGuildAsync()

### MemberService.cs

- Implement RemoveMemberAsync()

### Implement PetService.cs

- Implement a PetService.cs
- Add whitelist user Ids allowing people to add new images and pets
- Add list of pending animals to be confirmed
- Add list of valid pets to respond with

### UserService.cs

- Implement RemoveUserAsync()

## MutinyBot.cs

- Tidy up command errored

## Logging

- Implement consistent logging method through out program
- Log level definable via config
- Logg to console and log to file capabilities

## README.md

- Update with how to compile/run
- Update dependencies with additional NuGet links

## TODO.md

- Use repository issues and project board
- Promotion service?
- Welcome service?
- Redo and complete command descriptions