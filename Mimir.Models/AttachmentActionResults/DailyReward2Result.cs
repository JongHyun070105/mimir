using Bencodex.Types;
using Mimir.Models.Exceptions;
using Mimir.Models.Factories;
using Mimir.Models.Item;
using Nekoyume.Model.State;
using ValueKind = Bencodex.Types.ValueKind;

namespace Mimir.Models.AttachmentActionResults;

/// <summary>
/// <see cref="Nekoyume.Action.DailyReward2.DailyRewardResult"/>
/// </summary>
public record DailyReward2Result : AttachmentActionResult
{
    public Dictionary<Material, int> Materials { get; init; }
    public Guid Id { get; init; }

    public override IValue Bencoded => ((Dictionary)base.Bencoded)
        .Add("materials", new List(Materials
            .OrderBy(kv => kv.Key.Id)
            .Select(pair => (IValue)Dictionary.Empty
                .Add("material", pair.Key.Bencoded)
                .Add("count", pair.Value.Serialize()))))
        .Add("id", Id.Serialize());

    public DailyReward2Result(IValue bencoded) : base(bencoded)
    {
        if (bencoded is not Dictionary d)
        {
            throw new UnsupportedArgumentTypeException<ValueKind>(
                nameof(bencoded),
                [ValueKind.Dictionary],
                bencoded.Kind);
        }

        Materials = ((List)d["materials"])
            .Cast<Dictionary>()
            .ToDictionary(
                value => (Material)ItemFactory.Deserialize((Dictionary)value["material"]),
                value => value["count"].ToInteger());

        Id = d["id"].ToGuid();
    }
}
