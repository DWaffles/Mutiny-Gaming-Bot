# To-Do

## Common

### Configuration.cs

- Quit application after creating template config
- Better check on token?

## Database

### MutinyBotDbContext.cs

## Entities

### GuildEntity.cs

- Add bool value for enabling birthday messages
- Moderation log channel

### MemberEntity.cs

- Add Nationality flag emoji field?
	- enum for emojis?

### UserEntity.cs

- Add DateTime property for birthdays
- Add Nationality property

## Modules

### AdminModule.cs

- Implement AddMuteRole command to mute a user for a period of time and increment user TimesMuted 
- Implement GetInactiveUsers command using MemberEntity last message property

### CreatorModule.cs

### Implement GuildModule.cs

### MemeModule.cs

### MutinyGuildModule.cs

- Implement traingings/instructor command
- Implement mascot/cat command
- Implement arma mod command

### PetModule.cs 

- Implement list all pets in guild
- Implement list all pets in guild by member
- Implement using `[prefix]pet [user]` instead of `[prefix]pet pet [user]`
- Change `addpetowner` and `removepetowner` to not require creator permissions

### RoleModule.cs

- Simplify code for paginated embed

### UserModule.cs

- Implement salary field
	- "Take it up with my superior" - Kelsen
	- "I am the community -" - Prius

## Services

### EventService.cs

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

## Logging

- Implement consistent logging method through out program
- Log level definable via config

## README.md

- Update with how to compile/run
- Update dependencies with additional NuGet links

## TODO.md

- Use repository issues and project board
- Promotion service?
- Welcome service?
- Redo and complete command descriptions