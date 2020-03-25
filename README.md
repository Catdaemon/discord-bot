# .Net Core 3.1 Discord Bot

Monitors source servers and has 8ball.

## Usage

Add the following to environment or `config.json`:
- DISCORD_TOKEN
- COMMAND_PREFIX
- Servers

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
    ]
}
```