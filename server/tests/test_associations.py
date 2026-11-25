from server.market.storage import provision
from server.tests.contract_case import ContractCase


class AssociationTests(ContractCase):
    """Associations (PRD 8.1): at most 10 traders, a shared chat with a word filter and reports."""

    def test_an_association_holds_ten_traders_and_dissolves_when_the_last_leaves(self):
        self.assertEqual(self.order(self.seller, "found", name="x")[0], 400)
        self.assertEqual(self.order(self.seller, "found", name="Besi Tua Jaya")[0], 200)
        self.assertEqual(self.order(self.buyer, "found", name="besi tua jaya")[0], 409)
        self.assertEqual(self.order(self.seller, "found", name="Kedua")[0], 409)
        others = [provision(self.db, "Pedagang " + str(n)) for n in range(9)]
        for token in others:
            self.assertEqual(self.order(token, "join", name="Besi Tua Jaya")[0], 200)
        board = self.board(self.buyer)
        self.assertEqual(board["associations"], [{"name": "Besi Tua Jaya", "members": 10}])
        self.assertEqual(board["association"], {"name": "", "members": [], "messages": []})
        self.assertEqual(self.order(self.buyer, "join", name="Besi Tua Jaya")[0], 409)
        self.assertEqual(self.order(self.buyer, "leave")[0], 409)
        self.assertEqual(self.order(others[0], "leave")[0], 200)
        self.assertEqual(self.order(self.buyer, "join", name="Besi Tua Jaya")[0], 200)
        self.assertEqual(len(self.board(self.buyer)["association"]["members"]), 10)
        for token in [self.seller, self.buyer] + others[1:]:
            self.assertEqual(self.order(token, "leave")[0], 200)
        self.assertEqual(self.board(self.buyer)["associations"], [])

    def test_members_share_a_filtered_chat_and_can_report_a_line(self):
        self.order(self.seller, "found", name="Besi Tua Jaya")
        self.assertEqual(self.order(self.buyer, "say", text="halo")[0], 409)
        self.order(self.buyer, "join", name="Besi Tua Jaya")
        self.assertEqual(self.order(self.seller, "say", text="  ")[0], 400)
        self.assertEqual(self.order(self.seller, "say", text="x" * 121)[0], 400)
        self.assertEqual(self.order(self.seller, "say", text="Harga HMS naik, dasar GOBLOK")[0], 200)
        line = self.board(self.buyer)["association"]["messages"][0]
        self.assertEqual((line["name"], line["text"]), ("Sari", "Harga HMS naik, dasar ******"))
        self.assertEqual(self.order(self.seller, "report", message=line["id"])[0], 409)   # not his own line
        self.assertEqual(self.order(self.buyer, "report", message=line["id"])[0], 200)
        self.assertEqual(self.order(self.buyer, "report", message=line["id"])[0], 200)
        self.assertEqual(self.sql("SELECT body FROM reports"), [("Harga HMS naik, dasar ******",)])
        outsider = provision(self.db, "Tamu")
        self.assertEqual(self.board(outsider)["association"]["messages"], [])
        self.assertEqual(self.order(outsider, "report", message=line["id"])[0], 409)
