using BugPro;
using System.Linq;

namespace BugTests;

[TestClass]
public sealed class BugWorkflowTests
{
    [TestMethod]
    public void InitialState_IsNew()
    {
        var bug = new Bug();
        Assert.AreEqual(Bug.State.New, bug.CurrentState);
    }

    [TestMethod]
    public void StartTriage_FromNew_GoesToTriage()
    {
        var bug = new Bug();
        bug.StartTriage();
        Assert.AreEqual(Bug.State.Triage, bug.CurrentState);
    }

    [TestMethod]
    public void BeginFix_FromTriage_GoesToFixing()
    {
        var bug = new Bug();
        bug.StartTriage();
        bug.BeginFix();
        Assert.AreEqual(Bug.State.Fixing, bug.CurrentState);
    }

    [TestMethod]
    public void Resolve_FromFixing_GoesToReadyForTest()
    {
        var bug = new Bug();
        bug.StartTriage();
        bug.BeginFix();
        bug.Resolve();
        Assert.AreEqual(Bug.State.ReadyForTest, bug.CurrentState);
    }

    [TestMethod]
    public void VerifyOk_FromReadyForTest_GoesToClosed()
    {
        var bug = new Bug();
        bug.StartTriage();
        bug.BeginFix();
        bug.Resolve();
        bug.VerifyOk();
        Assert.AreEqual(Bug.State.Closed, bug.CurrentState);
    }

    [TestMethod]
    public void VerifyFail_FromReadyForTest_GoesToReopened()
    {
        var bug = new Bug();
        bug.StartTriage();
        bug.BeginFix();
        bug.Resolve();
        bug.VerifyFail();
        Assert.AreEqual(Bug.State.Reopened, bug.CurrentState);
    }

    [TestMethod]
    public void AnalyzeAgain_FromReopened_GoesToTriage()
    {
        var bug = new Bug();
        bug.StartTriage();
        bug.BeginFix();
        bug.Resolve();
        bug.VerifyFail();
        bug.AnalyzeAgain();
        Assert.AreEqual(Bug.State.Triage, bug.CurrentState);
    }

    [TestMethod]
    public void ReturnToOrigin_FromTriage_GoesToReturned()
    {
        var bug = new Bug();
        bug.StartTriage();
        bug.ReturnToOrigin();
        Assert.AreEqual(Bug.State.Returned, bug.CurrentState);
    }

    [TestMethod]
    public void Reopen_FromReturned_GoesToReopened()
    {
        var bug = new Bug();
        bug.StartTriage();
        bug.ReturnToOrigin();
        bug.Reopen();
        Assert.AreEqual(Bug.State.Reopened, bug.CurrentState);
    }

    [TestMethod]
    public void Reopen_FromClosed_GoesToReopened()
    {
        var bug = new Bug();
        bug.StartTriage();
        bug.BeginFix();
        bug.Resolve();
        bug.VerifyOk();
        bug.Reopen();
        Assert.AreEqual(Bug.State.Reopened, bug.CurrentState);
    }

    [TestMethod]
    public void AskMoreInfo_FromTriage_GoesToNeedMoreInfo()
    {
        var bug = new Bug();
        bug.StartTriage();
        bug.AskMoreInfo();
        Assert.AreEqual(Bug.State.NeedMoreInfo, bug.CurrentState);
    }

    [TestMethod]
    public void ProvideInfo_FromNeedMoreInfo_GoesToTriage()
    {
        var bug = new Bug();
        bug.StartTriage();
        bug.AskMoreInfo();
        bug.ProvideInfo();
        Assert.AreEqual(Bug.State.Triage, bug.CurrentState);
    }

    [TestMethod]
    public void ReturnToOrigin_FromNeedMoreInfo_GoesToReturned()
    {
        var bug = new Bug();
        bug.StartTriage();
        bug.AskMoreInfo();
        bug.ReturnToOrigin();
        Assert.AreEqual(Bug.State.Returned, bug.CurrentState);
    }

    [TestMethod]
    public void Defer_FromTriage_GoesToDeferred()
    {
        var bug = new Bug();
        bug.StartTriage();
        bug.Defer();
        Assert.AreEqual(Bug.State.Deferred, bug.CurrentState);
    }

    [TestMethod]
    public void ResumeWork_FromDeferred_GoesToFixing()
    {
        var bug = new Bug();
        bug.StartTriage();
        bug.Defer();
        bug.ResumeWork();
        Assert.AreEqual(Bug.State.Fixing, bug.CurrentState);
    }

    [TestMethod]
    public void Defer_FromFixing_GoesToDeferred()
    {
        var bug = new Bug();
        bug.StartTriage();
        bug.BeginFix();
        bug.Defer();
        Assert.AreEqual(Bug.State.Deferred, bug.CurrentState);
    }

    [TestMethod]
    public void RouteToOtherProduct_FromTriage_GoesToOtherProduct()
    {
        var bug = new Bug();
        bug.StartTriage();
        bug.RouteToOtherProduct();
        Assert.AreEqual(Bug.State.OtherProduct, bug.CurrentState);
    }

    [TestMethod]
    public void ReturnToOrigin_FromOtherProduct_GoesToReturned()
    {
        var bug = new Bug();
        bug.StartTriage();
        bug.RouteToOtherProduct();
        bug.ReturnToOrigin();
        Assert.AreEqual(Bug.State.Returned, bug.CurrentState);
    }

    [TestMethod]
    public void InvalidTransition_BeginFix_FromNew_Throws()
    {
        var bug = new Bug();
        Assert.ThrowsException<InvalidOperationException>(() => bug.BeginFix());
    }

    [TestMethod]
    public void InvalidTransition_VerifyOk_FromFixing_Throws()
    {
        var bug = new Bug();
        bug.StartTriage();
        bug.BeginFix();
        Assert.ThrowsException<InvalidOperationException>(() => bug.VerifyOk());
    }

    [TestMethod]
    public void InvalidTransition_ProvideInfo_FromNew_Throws()
    {
        var bug = new Bug();
        Assert.ThrowsException<InvalidOperationException>(() => bug.ProvideInfo());
    }

    [TestMethod]
    public void PermittedTriggers_InTriage_ContainsBeginFix()
    {
        var bug = new Bug();
        bug.StartTriage();
        Assert.IsTrue(bug.PermittedTriggers.Contains(Bug.Trigger.BeginFix));
    }
}
