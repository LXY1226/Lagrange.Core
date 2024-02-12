using System.Runtime.InteropServices.JavaScript;
using System.Text.Json.Serialization;
using Lagrange.Core.Message;
using Lagrange.Core.Message.Entity;

namespace Lagrange.OneBot.Message.Entity;

[Serializable]
public partial class ImageSegment(string url)
{
    public ImageSegment() : this("") { }

    [JsonPropertyName("file")][CQProperty] public string File { get; set; } = url;
    [JsonPropertyName("md5")][CQProperty] public string MD5 { get; set; } = "";
    [JsonPropertyName("sha1")][CQProperty] public string SHA1 { get; set; } = "";
    [JsonPropertyName("size")][CQProperty] public int Size { get; set; } = -1;

    [JsonPropertyName("url")] public string Url { get; set; } = url;
}

[SegmentSubscriber(typeof(ImageEntity), "image")]
public partial class ImageSegment : SegmentBase
{
    public override void Build(MessageBuilder builder, SegmentBase segment)
    {
        if (segment is ImageSegment imageSegment and not { File: "" } && CommonResolver.Resolve(imageSegment.File) is { } image)
        {
            builder.Image(image);
        }
    }

    public override SegmentBase FromEntity(MessageChain chain, IMessageEntity entity)
    {
        if (entity is not ImageEntity imageEntity) throw new ArgumentException("Invalid entity type.");

        return new ImageSegment(imageEntity.ImageUrl)
        {
            MD5 = imageEntity.FileMd5,
            SHA1 = imageEntity.FileSha1,
            Size = (int)imageEntity.ImageSize,
        };
    }
}