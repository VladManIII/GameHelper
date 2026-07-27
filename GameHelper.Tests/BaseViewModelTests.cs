using GameHelper.Pages;

namespace GameHelper.Tests;

public class BaseViewModelTests
{
    private sealed class TestViewModel : BaseViewModel
    {
        public int ActivatedCount;
        public int ClosedCount;
        public int VisibilityChangedCount;

        protected override void OnActivated() => ActivatedCount++;
        protected override void OnClosed() => ClosedCount++;
        protected override void OnVisibilityChanged() => VisibilityChangedCount++;
    }

    [Fact]
    public void Activated_InvokesOnActivated()
    {
        var viewModel = new TestViewModel();

        ((IViewModelLifecycle)viewModel).Activated();

        Assert.Equal(1, viewModel.ActivatedCount);
    }

    [Fact]
    public void Closed_InvokesOnClosed()
    {
        var viewModel = new TestViewModel();

        ((IViewModelLifecycle)viewModel).Closed();

        Assert.Equal(1, viewModel.ClosedCount);
    }

    [Fact]
    public void VisibilityChanged_InvokesOnVisibilityChanged()
    {
        var viewModel = new TestViewModel();

        ((IViewModelLifecycle)viewModel).VisibilityChanged();

        Assert.Equal(1, viewModel.VisibilityChangedCount);
    }
}
