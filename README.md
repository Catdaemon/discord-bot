# .Net Core 3.1 Discord Bot

Monitors source servers, logs chat, produces stats, and has !8ball.

## Usage

Add the following to environment or `config.json`:
- DISCORD_TOKEN
- COMMAND_PREFIX
- Servers
- ExcludeWords

If you want to persist chat logs and are using docker, map the /db folder.

## config.json example
```json
{
    "DISCORD_TOKEN": "dsadadasdasd",
    "COMMAND_PREFIX": "!",

    "Servers": [
        {
            "Name": "Gmod Prop Hunt",
            "Address": "ip.or.dns",
            "Port": 27015,
            "RCONPassword": "strongpassword"
        },
        {
            "Name": "Gmod TTT",
            "Address": "ip.or.dns",
            "Port": 27016,
            "RCONPassword": "strongpassword"
        }
    ],

    "ExcludeWords": [
        "the",
        "and",
        "a",
        "an",
        "are",
        "at",
        "be",
        "by",
        "for",
        "from"
    ]
}
```