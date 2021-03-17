# To-Do

## Database

### MutinyBotDbContext.cs

- Add Users DbSet
- Find a less lazy and much more robust way of implementing migrations for production settings

## Entities

### GuildEntity.cs

- Add bool value for enabling birthday messages
- Remove GuildName or have it used for console messages

### MemberEntity.cs

- Add DateTime property for last message
- Add int TimesMuted propery

### UserEntity.cs

- Add DateTime property for birthdays
- Add Nationality property
- Add bool IsBotBanned property

## Modules

### AdminModule.cs

- Implement AddMuteRole command to mute a user for a period of time and increment user TimesMuted 
- Implement GetInactiveUsers command using MemberEntity last message property

### CreatorModule.cs

- Add BotBan command

### MutinyGuildModule.cs

- Implement traingings command
- Implement mascot/cat command
- Implement mod command

### UserModule.cs

- Use Humanize for date time
- Implement salary field

## Services

### EventService.cs

- Fix problem regarding OnGuildAvailable() and OnGuildJoined() taking more than 1 second from UpdateGuild() for guilds >100 member count
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