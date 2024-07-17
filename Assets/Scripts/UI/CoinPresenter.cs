using KartDemo;
using KartDemo.MVP;

public class CoinPresenter : Presenter<CoinView, PlayerTrack>
{
    //public CoinPresenter(CoinView coinView, CoinModel coinModel) : base(coinView, coinModel)
    //{

    //}

    public override void Init()
    {
        model.OnCoinCollect += Collect;
    }

    public void Collect()
    {
        view.SetCoin(model.CoinAmount());
    }
}