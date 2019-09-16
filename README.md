# UndergroundIRO.BestChangeParserKit - simple parser for https://www.bestchange.ru exchangers catalog

Powered by HtmlAgilityPack.

Parse https://www.bestchange.ru/ currency pairs tables to models:

```json
  {
    "ExchangerTitle": "Касса",
    "ExchangerUrl": "https://www.bestchange.ru/click.php?id=522&from=93&to=172&url=1",
    "CoinNameFrom": "BTC",
    "CoinNameTo": "BCH",
    "Rate": 29.45771336,
    "Min_FromCoin": 0.001,
    "Fund": 8.57,
    "IsFloatingRate": false,
    "IsManual": true,
    "NeedVerification": false
  }
```