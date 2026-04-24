using Stateless;

namespace BugPro;

public static class Program
{
    public static void Main()
    {
        var bug = new Bug();

        Console.WriteLine($"Initial: {bug.CurrentState}");
        bug.StartTriage();
        Console.WriteLine($"After triage: {bug.CurrentState}");
        bug.BeginFix();
        Console.WriteLine($"After begin fix: {bug.CurrentState}");
        bug.Resolve();
        Console.WriteLine($"After resolve: {bug.CurrentState}");
        bug.VerifyOk();
        Console.WriteLine($"After verify OK: {bug.CurrentState}");
    }
}

public sealed class Bug
{
    public enum State
    {
        New,
        Triage,
        Fixing,
        NeedMoreInfo,
        Deferred,
        OtherProduct,
        ReadyForTest,
        Returned,
        Reopened,
        Closed
    }

    public enum Trigger
    {
        StartTriage,
        BeginFix,
        AskMoreInfo,
        ProvideInfo,
        Defer,
        ResumeWork,
        RouteToOtherProduct,
        ReturnToOrigin,
        Resolve,
        VerifyOk,
        VerifyFail,
        Reopen,
        AnalyzeAgain
    }

    private readonly StateMachine<State, Trigger> _machine;

    public Bug()
    {
        _machine = new StateMachine<State, Trigger>(State.New);

        _machine.Configure(State.New)
            .Permit(Trigger.StartTriage, State.Triage);

        _machine.Configure(State.Triage)
            .Permit(Trigger.BeginFix, State.Fixing)
            .Permit(Trigger.AskMoreInfo, State.NeedMoreInfo)
            .Permit(Trigger.Defer, State.Deferred)
            .Permit(Trigger.RouteToOtherProduct, State.OtherProduct)
            .Permit(Trigger.ReturnToOrigin, State.Returned);

        _machine.Configure(State.NeedMoreInfo)
            .Permit(Trigger.ProvideInfo, State.Triage)
            .Permit(Trigger.ReturnToOrigin, State.Returned);

        _machine.Configure(State.Deferred)
            .Permit(Trigger.ResumeWork, State.Fixing)
            .Permit(Trigger.ReturnToOrigin, State.Returned);

        _machine.Configure(State.OtherProduct)
            .Permit(Trigger.ReturnToOrigin, State.Returned);

        _machine.Configure(State.Fixing)
            .Permit(Trigger.Resolve, State.ReadyForTest)
            .Permit(Trigger.AskMoreInfo, State.NeedMoreInfo)
            .Permit(Trigger.Defer, State.Deferred)
            .Permit(Trigger.RouteToOtherProduct, State.OtherProduct);

        _machine.Configure(State.ReadyForTest)
            .Permit(Trigger.VerifyOk, State.Closed)
            .Permit(Trigger.VerifyFail, State.Reopened);

        _machine.Configure(State.Returned)
            .Permit(Trigger.Reopen, State.Reopened);

        _machine.Configure(State.Reopened)
            .Permit(Trigger.AnalyzeAgain, State.Triage)
            .Permit(Trigger.ReturnToOrigin, State.Returned);

        _machine.Configure(State.Closed)
            .Permit(Trigger.Reopen, State.Reopened);
    }

    public State CurrentState => _machine.State;

    public IEnumerable<Trigger> PermittedTriggers => _machine.PermittedTriggers;

    public void StartTriage() => _machine.Fire(Trigger.StartTriage);
    public void BeginFix() => _machine.Fire(Trigger.BeginFix);
    public void AskMoreInfo() => _machine.Fire(Trigger.AskMoreInfo);
    public void ProvideInfo() => _machine.Fire(Trigger.ProvideInfo);
    public void Defer() => _machine.Fire(Trigger.Defer);
    public void ResumeWork() => _machine.Fire(Trigger.ResumeWork);
    public void RouteToOtherProduct() => _machine.Fire(Trigger.RouteToOtherProduct);
    public void ReturnToOrigin() => _machine.Fire(Trigger.ReturnToOrigin);
    public void Resolve() => _machine.Fire(Trigger.Resolve);
    public void VerifyOk() => _machine.Fire(Trigger.VerifyOk);
    public void VerifyFail() => _machine.Fire(Trigger.VerifyFail);
    public void Reopen() => _machine.Fire(Trigger.Reopen);
    public void AnalyzeAgain() => _machine.Fire(Trigger.AnalyzeAgain);
}
