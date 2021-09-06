module FSharpDiscordBot.Program

open System
open System.Threading.Tasks
open Discord
open Discord.WebSocket

let log (msg: LogMessage) =
    printfn $"%s{msg.ToString()}"
    Task.CompletedTask

let logFunc = Func<LogMessage, Task>(log)

let msgReceived (msg: SocketMessage) =
    
    match msg.Content with
    | "!ping" ->
        msg.Channel.SendMessageAsync("pong!", messageReference = msg.Reference)
        |> Async.AwaitTask
        |> Async.RunSynchronously
        |> ignore
    | _ -> () // shut up compiler warnings
    
    Task.CompletedTask

let msgReceivedFunc = Func<SocketMessage, Task>(msgReceived)

let ready (client: DiscordSocketClient) () =
    
    client.SetGameAsync("not f# bots having sad looking code", ``type`` = ActivityType.Watching)
    |> Async.AwaitTask |> Async.RunSynchronously
    
    Task.CompletedTask

let readyFunc client = Func<Task>(ready client)

let botMain token = async {
        let client = new DiscordSocketClient()
        
        client.add_Log logFunc
        client.add_MessageReceived msgReceivedFunc
        client.add_Ready (readyFunc client)
        
        client.LoginAsync(TokenType.Bot, token)
        |> Async.AwaitTask |> Async.RunSynchronously
        
        client.StartAsync()
        |> Async.AwaitTask |> Async.RunSynchronously
        
        // idk why i need this but the discord.net example has it so sure
        Task.Delay(-1)
        |> Async.AwaitTask |> Async.RunSynchronously
    }


[<EntryPoint>]
let main _ =
    
    let token = Environment.GetEnvironmentVariable("FSHARP_BOT_TOKEN")
    
    botMain token
        |> Async.RunSynchronously
    
    0