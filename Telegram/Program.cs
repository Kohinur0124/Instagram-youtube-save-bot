using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using VideoLibrary;

var botClient = new TelegramBotClient("6023621119:AAHQMzlIsp63leprBkqEqZpy2RSpfChzvEc");

using CancellationTokenSource cts = new();

// StartReceiving does not block the caller thread. Receiving is done on the ThreadPool.
ReceiverOptions receiverOptions = new()
{
    AllowedUpdates = Array.Empty<UpdateType>() // receive all update types except ChatMember related updates
};

botClient.StartReceiving(
    updateHandler: HandleUpdateAsync,
    pollingErrorHandler: HandlePollingErrorAsync,
    receiverOptions: receiverOptions,
    cancellationToken: cts.Token
);



var me = await botClient.GetMeAsync();

Console.WriteLine($"Start listening for @{me.Username}");
Console.ReadLine();

// Send cancellation request to stop bot
cts.Cancel();

async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
{
    
    // Only process Message updates: https://core.telegram.org/bots/api#message
    if (update.Message is not { } message)
        return;
    // Only process text messages
    if (message.Text is not { } messageText)
        return;

    var chatId = message.Chat.Id;

    Console.WriteLine($"Received a '{messageText}' message in chat {chatId}.");


    if (message.Text == "/start")
    {
        Message message1 = await botClient.SendStickerAsync(
        chatId: chatId,
        sticker: InputFile.FromUri("https://img-08.stickers.cloud/packs/7f87fca0-633f-4695-8076-dff18b4ce0cc/webp/28e95655-ddd8-4f7c-a958-e7814d8dff20.webp\r\n"),
        cancellationToken: cancellationToken);

    }
    else if (message.Text.Contains("www.instagram.com"))
    {
        try
        {
            var mes = message.Text.Replace("www.", "dd");
            Message message1 = await botClient.SendVideoAsync(
                chatId: chatId,
                InputFile.FromUri($"{mes}"),
                cancellationToken: cancellationToken); ;

        }
        catch
        {
            try
            {
                var mes = message.Text/*.Replace("www.", "dd")*/;
                Message message1 = await botClient.SendPhotoAsync(
                 chatId: chatId,
                 photo: InputFile.FromUri($"{mes}"),
                 caption: "<b>Ara bird</b>. <i>Source</i>: <a href=\"https://pixabay.com\">Pixabay</a>",
                 parseMode: ParseMode.Html,
                 cancellationToken: cancellationToken);

            }
            catch
            {
                Message sentMessage = await botClient.SendTextMessageAsync(
                    chatId: chatId,
                    text: "Fileni yuklashni iloji bo`lmadi !!!",
                    cancellationToken: cancellationToken
                );
                Message message1 = await botClient.SendStickerAsync(
                   chatId: chatId,
                   sticker: InputFile.FromUri("https://img-08.stickers.cloud/packs/7f87fca0-633f-4695-8076-dff18b4ce0cc/webp/8ec659de-b0e7-4608-8028-cfea0ae49b3c.webp"),
                   cancellationToken: cancellationToken
                );

            }

        }

    }
    else if (message.Text.Contains("you"))
    {
        YouTube youTube = new YouTube();
        var video = youTube.GetVideo(message.Text);

        Message message1 = await botClient.SendVideoAsync(
                chatId: chatId,
                InputFile.FromStream(video.Stream()),
                cancellationToken: cancellationToken); ;


    }
    else
    {

        Message sentMessage = await botClient.SendTextMessageAsync(
                    chatId: chatId,
                    text: "Instagram linkni tashlang  !!!",
                    cancellationToken: cancellationToken
                );
        Message message1 = await botClient.SendStickerAsync(
                  chatId: chatId,
                  sticker: InputFile.FromUri("https://stickerswiki.ams3.cdn.digitaloceanspaces.com/GoRobot/6173681.512.webp"),
                  cancellationToken: cancellationToken
               );

    }

    /*if(message.Text == "stikker")
    {
        Message message1= await botClient.SendStickerAsync(
        chatId: chatId,
        sticker: InputFile.FromUri("https://chpic.su/_data/stickers/k/kolobkiOdnoklassniki/kolobkiOdnoklassniki_001.webp"),
        cancellationToken: cancellationToken);

    }
    else if (message.Text == "video"){
        Message message1 = await botClient.SendVideoAsync(
        chatId: chatId,
        video: InputFile.FromUri("https://media.istockphoto.com/id/1298885769/video/abstract-digital-connection-moving-dots-lines-and-numbers-abstract-dof-blurred-counting.mp4?s=mp4-640x640-is&k=20&c=fvGeCwT5N7CzVeeIvwPgkDDTDPEC30rJpoJo3VRbBY8="),
        cancellationToken: cancellationToken);
    }
    else if(message.Text == "Photo")
    {
        Message message1 = await botClient.SendVideoAsync(
        chatId: chatId,
        video: InputFile.FromUri("https://img.freepik.com/premium-photo/futuristic-abstract-love-cloud-landscape-generative-ai_372999-12223.jpg"),
        cancellationToken: cancellationToken);
    }*/
}

Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
{
    var ErrorMessage = exception switch
    {
        ApiRequestException apiRequestException
            => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
        _ => exception.ToString()
    };

    Console.WriteLine(ErrorMessage);
    return Task.CompletedTask;
}
