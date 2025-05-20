- mam DataSource

  - definuju pres interface / abstract classu
  - dataSource pak ma konkretni implementace - API, download, scrape
  - taky moznost vizualizace
  - pro jednotlive implementace mohu delat jeste specificke tridy - treba pro crawlera svuj apod.

- potrebuju nejaky service na registrovani DataSourcu

  - mozna pres factory pattern

- DataSource se pak ulozi do JSONu
  -> Budu potrebovat nejaky JSON reader / writer - serializer / deserializer pro jednotlive DataSources

- z jednotlivch DataSources muzu skladat dashboards
- takze neco k registrovani noveho dashboardu
- opet definici dashboardu ulozim do JSONu s jeho data sources - nejak chytre serializovat / deserializovat

- nejak vizualizovat ty dashboardy?
- pro vsechno modely
