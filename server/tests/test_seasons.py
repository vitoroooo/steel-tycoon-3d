from server.market.storage import provision
from server.tests.contract_case import DAY, ContractCase

REAL_DAY = 86400


class SeasonTests(ContractCase):
    """Seasons (PRD 11.3): 90 real days, Pekan Tutup Buku from day 76, then everything moves into Grup Usaha."""

    def test_the_calendar_counts_ninety_days_and_the_closing_week_bars_long_contracts(self):
        board = self.board(self.seller)
        self.assertEqual((board["season"], board["city"], board["seasonDay"], board["seasonDaysLeft"], board["closing"]),
                         (1, "Sukabaja", 1, 90, False))
        self.now += 75 * REAL_DAY
        board = self.board(self.seller)
        self.assertEqual((board["seasonDay"], board["seasonDaysLeft"], board["closing"]), (76, 15, True))
        self.now += 14 * REAL_DAY + REAL_DAY // 2      # half a real day left: three game days
        offer = dict(buyer="Hasan", kg=10, price=self.board(self.seller)["reference"])
        self.assertEqual(self.order(self.seller, "offer", days=4, **offer)[0], 409)
        self.assertEqual(self.order(self.seller, "offer", days=3, **offer)[0], 200)
        self.now += DAY                                 # the buyer waited too long: three days no longer fit
        number = self.board(self.buyer)["contracts"][0]["id"]
        self.assertEqual(self.order(self.buyer, "accept", contract=number)[0], 409)

    def test_closing_keeps_the_business_as_group_history_and_everyone_starts_again(self):
        self.order(self.seller, "found", name="Besi Tua Jaya")
        self.sql("UPDATE traders SET cash=300000000 WHERE name='Sari'")
        self.order(self.seller, "promote")
        self.running(days=10)
        self.order(self.seller, "sell", kg=5, price=self.price)
        self.assertEqual(self.order(self.seller, "bekal", picks="triseda")[0], 409)   # no season closed yet
        self.now += 90 * REAL_DAY
        sari, hasan = self.board(self.seller), self.board(self.buyer)
        self.assertEqual((sari["season"], sari["city"], sari["seasonDay"]), (2, "Tanjung Karat", 1))
        self.assertEqual((sari["cash"], sari["stock"], sari["caste"], sari["lots"]), (200000, 20, 1, []))
        self.assertEqual((hasan["cash"], hasan["contracts"][0]["status"], hasan["contracts"][0]["penalty"]),
                         (200000, "broken", 0))
        self.assertEqual(sari["association"]["name"], "Besi Tua Jaya")                # the association stays
        past = sari["group"]["history"][0]
        self.assertEqual((past["season"], past["city"], past["rank"]), (1, "Sukabaja", 1))
        self.assertEqual(hasan["group"]["history"][0]["rank"], 2)
        self.assertEqual(past["points"], past["value"] // 1000000)
        self.assertGreaterEqual(past["value"], 50000000)
        self.assertEqual((sari["group"]["points"], sari["group"]["level"]), (past["points"], 1))
        self.now += 5
        self.assertEqual(self.board(self.seller)["group"]["history"], [past])         # closed once only

    def test_a_veteran_picks_three_legacies_his_group_level_has_opened(self):
        self.sql("UPDATE traders SET cash=450000000 WHERE name='Sari'")
        self.now += 90 * REAL_DAY
        group = self.board(self.seller)["group"]
        self.assertEqual((group["level"], group["choose"], group["picks"]), (3, True, []))
        self.assertEqual(self.board(self.buyer)["group"]["level"], 1)
        self.assertEqual(self.order(self.buyer, "bekal", picks="antrean")[0], 409)    # needs group level 2
        self.assertEqual(self.order(self.seller, "bekal", picks="triseda,pt,sortir,antrean")[0], 400)
        self.assertEqual(self.order(self.seller, "bekal", picks="triseda,triseda")[0], 400)
        self.assertEqual(self.order(self.seller, "bekal", picks="triseda,pt,antrean")[0], 200)
        self.assertEqual(self.order(self.seller, "bekal", picks="sortir")[0], 409)    # once a season
        group = self.board(self.seller)["group"]
        self.assertEqual((group["picks"], group["choose"]), (["triseda", "pt", "antrean"], False))
        # "antrean offline +2 jam": away for a week, he collects 50 game hours, the other trader 48
        before = self.board(self.seller)["stock"], self.board(self.buyer)["stock"]
        self.now += 7 * REAL_DAY
        self.assertEqual((self.board(self.seller)["stock"] - before[0], self.board(self.buyer)["stock"] - before[1]),
                         (50, 48))

    def test_a_trader_who_joins_mid_season_gets_a_catch_up_stock(self):
        self.sql("UPDATE city SET opened_at=opened_at-30*86400")
        late = provision(self.db, "Telat")
        # 30 real days are 4,320 game hours of collecting 1 kg; he is given half of it on top of the 20 kg
        self.assertEqual(self.board(late)["stock"], 20 + 2160)
