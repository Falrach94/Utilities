namespace GameUtilities.GameMode.RuleBased.Rules.RuleSet
{
    public delegate object GameActionHandler<TModel>(TModel model);
    public interface IGameAction<TModelData>
    {
        GameActionHandler<TModelData> Handler { get; }
    }
}