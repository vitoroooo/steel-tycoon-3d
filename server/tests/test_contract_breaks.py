from server.market.moderation import moderate
from server.tests.contract_case import DAY, ContractCase


class ContractBreakTests(ContractCase):
    """Ending a contract early (PRD C.2): 10% of what is left goes to the other side."""

    def test_cancelling_returns_the_lock_and_pays_ten_percent_of_what_is_left(self):
        number = self.running(days=10)
        self.now += DAY
        day = 10 * self.price
        status, ended = self.order(self.buyer, "end", contract=number)
        self.assertEqual((status, ended["penalty"]), (200, 9 * day // 10))
        self.assertEqual(self.board(self.buyer)["cash"], 1000000 - day - 9 * day // 10)
        self.assertEqual(self.board(self.seller)["cash"], 1000000 + day - (day * 2 + 50) // 100 + 9 * day // 10)
        self.assertEqual(self.order(self.buyer, "end", contract=number)[0], 409)
        self.running(days=3)
        moderate(self.db, "Sari", True)   # the operator suspends the seller: the lock goes back, nobody pays a penalty
        after = self.board(self.buyer)
        self.assertEqual((after["contracts"][0]["status"], after["contracts"][0]["penalty"]), ("broken", 0))
        self.assertEqual(after["cash"], 1000000 - day - 9 * day // 10)

    def test_a_seller_without_stock_or_a_buyer_without_money_breaks_it_and_pays(self):
        self.running(kg=150, days=3)   # 100 kg in stock and 24 collected a day: he cannot deliver 150
        day = 150 * self.price
        self.now += DAY
        buyer = self.board(self.buyer)
        self.assertEqual(buyer["contracts"][0]["status"], "broken")
        self.assertEqual(buyer["cash"], 1000000 + 3 * day // 10)
        self.sql("UPDATE contracts SET status='withdrawn'")
        number = self.running(kg=10, days=3)
        self.sql("UPDATE traders SET cash=5 WHERE name='Hasan'")   # the first day is locked, the second cannot be
        self.now += DAY
        buyer = self.board(self.buyer)
        self.assertEqual((buyer["cash"], buyer["contracts"][0]["status"]), (0, "broken"))
        self.assertEqual(self.sql(f"SELECT penalty FROM contracts WHERE id={number}"), [(5,)])
