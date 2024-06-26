namespace cli_life;

[TestClass]
public class UnitTest1
{
    [TestMethod]
    public void TestMethod1()
    {
        Board board = new Board(50, 100, 1);
        Assert.AreEqual(100, board.Height);
        Assert.AreEqual(50, board.Width);
        Assert.AreEqual(1, board.CellSize);
    }
    [TestMethod]
    public void TestMethod2()
    {
        Board board = new Board(50, 100, 1);
        Assert.AreEqual(0, board.Generation);
        Assert.AreEqual(0, board.states.Count);
    }
    [TestMethod]
    public void TestMethod3()
    {
        Board board = Board.WithSettings("../../../settings.json");
        Assert.AreEqual(20, board.Height);
        Assert.AreEqual(100, board.Width);
    }
    [TestMethod]
    public void TestMethod4()
    {
        Board board = new Board("../../../stable.txt");
        Assert.AreEqual(20, board.Rows);
        Assert.AreEqual(100, board.Columns);
    }
    [TestMethod]
    public void TestMethod5()
    {
        Board board = new Board("../../../stable.txt");
        for (int i = 0; i < 3; i++)
        {
            board.Advance();
            board.Stable();
        }
        Assert.AreEqual(true, board.Stable());
    }
    [TestMethod]
    public void TestMethod6()
    {
        Board board = new Board("../../../stable.txt");
        Assert.AreEqual(95, board.Cells.Cast<Cell>().Where(x => x.IsAlive).Count());
    }
    [TestMethod]
    public void TestMethod7()
    {
        Board board = new Board("../../../stable.txt");
        int[,] elem = new int[,]{{0,0,0,0}, {0,1,1,0}, {0,1,1,0}, {0,0,0,0}};
        Assert.AreEqual(7, board.CountElement(elem));
    }
    [TestMethod]
    public void TestMethod8()
    {
        Board board = new Board(50, 100, 1);
        board.Save("testsave.txt");
        Assert.AreEqual(true, File.Exists("testsave.txt"));
    }
    [TestMethod]
    public void TestMethod9()
    {
        Board board = new Board("../../../stable.txt");
        int[,] elem = new int[,]{{0,0,0,0}, {0,1,1,0}, {0,1,1,0}, {0,0,0,0}};
        Assert.AreEqual(true, elem.SequenceEquals(board.Submatrix(89, 5, 4, 4)));
    }
    [TestMethod]
    public void TestMethod10()
    {
        Board board = new Board(50, 100, 1, 1);
        board.Advance();
        Assert.AreEqual(0, board.Cells.Cast<Cell>().Where(x => x.IsAlive).Count());
    }
    [TestMethod]
    public void TestMethod11()
    {
        Board board = new Board(50, 100, 1, 0.5);
        int live1 = board.Cells.Cast<Cell>().Where(x => x.IsAlive).Count();
        for (int i = 0; i < 50; i++)
            board.Advance();
        int live2 = board.Cells.Cast<Cell>().Where(x => x.IsAlive).Count();
        Assert.AreEqual(true, live1 >= live2);
    }
    [TestMethod]
    public void TestMethod12()
    {
        Board board = new Board("../../../stable.txt");
        int[,] elem = new int[,]{{0,0,0,0,0,0}, {0,0,1,1,0,0}, {0,1,0,0,1,0}, {0,0,1,1,0,0}, {0,0,0,0,0,0}};
        Assert.AreEqual(6, board.CountElement(elem));
    }
    [TestMethod]
    public void TestMethod13()
    {
        int[,] a1 = new int[,] {
            {1, 2, 3}, {4, 5, 6} 
        };
        int[,] a2 = new int[,] {
            {1, 2, 3}, {4, 5, 6} 
        };
        Assert.AreEqual(true, a1.SequenceEquals(a2));
    }
    [TestMethod]
    public void TestMethod14()
    {
        int[,] a1 = new int[,] {
            {1, 2, 3}, {4, 5, 6} 
        };
        int[,] a2 = new int[,] {
            {4, 1}, {5, 2}, {6, 3}
        };
        Assert.AreEqual(true, a2.SequenceEquals(a1.Rotate90()));
    }
    [TestMethod]
    public void TestMethod15()
    {
        Board board = new Board("../../../stable.txt");
        int[,] elem = new int[,]{{0,0,0,0,0}, {0,0,1,0,0}, {0,1,0,1,0}, {0,1,1,0,0}, {0,0,0,0,0}};
        Assert.AreEqual(3, board.CountElement(elem));
    }
}