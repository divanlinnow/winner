using System.Text;

static class Derivco
{
    public static void Main(String[] args)
    {
        const int ace = 1;
        const int jack = 11;
        const int queen = 12;
        const int king = 13;

        const int spades = 4;
        const int hearts = 3;
        const int diamonds = 2;
        const int clubs = 1;

        bool hasInput = false;
        bool hasOutput = false;

        string inputPath = string.Empty;
        string outputPath = string.Empty;

        List<Tuple<string, int, int>> playerScores = new();

        for (int i = 0; i < args.Length; i++)
        {
            if (args[i] == "--in")
            {
                inputPath = Path.Combine(Directory.GetCurrentDirectory(), args[i + 1]);
                hasInput = File.Exists(inputPath);
            }

            if (args[i] == "--out")
            {
                outputPath = Path.Combine(Directory.GetCurrentDirectory(), args[i + 1]);
                hasOutput = File.Exists(outputPath);
            }
        }

        if (hasInput && !string.IsNullOrEmpty(outputPath))
        {
            string[] lines = File.ReadAllLines(inputPath);

            if (lines.Length > 5)
            {
                OutPutError(outputPath);
                return;
            }

            foreach (string line in lines)
            {
                string playerName = line.Split(':').First();
                string[] cards = line.Split(':').Last().Split(',').ToArray();

                int playerScore = 0;
                int playerSuitScore = 0;

                foreach (string card in cards)
                {
                    // Calculate card score
                    string score = card.Remove(card.Length - 1).ToString();
                    string suit = card[card.Length - 1].ToString();

                    bool isNumeric = int.TryParse(score, out int points);

                    if (isNumeric)
                    {
                        playerScore += points;
                    }
                    else
                    {
                        switch (score.ToUpper())
                        {
                            case "A":
                                playerScore += ace;
                                break;
                            case "K":
                                playerScore += king;
                                break;
                            case "Q":
                                playerScore += queen;
                                break;
                            case "J":
                                playerScore += jack;
                                break;
                            default:
                                OutPutError(outputPath);
                                return;
                        }
                    }

                    // Calculate suit score
                    switch (suit.ToUpper())
                    {
                        case "S":
                            playerSuitScore += spades;
                            break;
                        case "H":
                            playerSuitScore += hearts;
                            break;
                        case "D":
                            playerSuitScore += diamonds;
                            break;
                        case "C":
                            playerSuitScore += clubs;
                            break;
                        default:
                            OutPutError(outputPath);
                            return;
                    }
                }

                playerScores.Add(Tuple.Create(playerName, playerScore, playerSuitScore));
            }

            var winners = playerScores.Where(x => x.Item2 == playerScores.Select(s => s.Item2).Max()).ToList();

            if (winners.Count == 1)
            {
                OutPutSingleWinner(winners[0], outputPath);
            }
            else if (winners.Count > 1)
            {
                var suitWinners = winners.Where(w => w.Item3 == winners.Select(s => s.Item3).Max()).ToList();

                if (suitWinners.Count == 1)
                {
                    OutPutSingleWinner(suitWinners[0], outputPath);
                }
                else if (suitWinners.Count > 1)
                {
                    OutPutMultipleWinners(suitWinners, outputPath);
                }
            }
        }
    }

    static void OutPutSingleWinner(Tuple<string, int, int> winners, string outputPath)
    {
        using StreamWriter file = new StreamWriter(outputPath, false, Encoding.UTF8);
        file.WriteLine(winners.Item1 + ":" + winners.Item2);
        file.Close();
    }

    static void OutPutMultipleWinners(List<Tuple<string, int, int>> winners, string outputPath)
    {
        using StreamWriter file = new StreamWriter(outputPath, false, Encoding.UTF8);

        winners.ForEach(winner =>
        {
            if (winner != winners.Last())
            {
                file.Write(winner.Item1 + ",");
            }
            else
            {
                int score = winner.Item2 + winner.Item3;
                file.Write(winner.Item1 + ":" + score);

            }
        });

        file.Close();
    }

    static void OutPutError(string outputPath)
    {
        using StreamWriter file = new StreamWriter(outputPath, false, Encoding.UTF8);

        file.WriteLine("ERROR");

        file.Close();
    }
}