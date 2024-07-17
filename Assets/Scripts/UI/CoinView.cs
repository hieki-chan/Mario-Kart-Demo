using KartDemo.MVP;
using TMPro;

public class CoinView : View
{
    public TextMeshProUGUI coinText;

    public void SetCoin(int amount)
    {
        coinText.text = amount.ToString();
    }
}