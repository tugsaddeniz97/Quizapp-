import { useState, useEffect } from "react";
import { createPlayer, getPlayers, getQuestions } from "./services/api";
import "./App.css";
import QuizGame from "./components/QuizGame.jsx";
import LoginPage from "./components/LoginPage.jsx";
import Leaderboard from "./components/Leaderboard.jsx";
function App() {
  const [questions, setQuestions] = useState([]);
  const [players, setPlayers] = useState([]);
  const [currentPlayer, setCurrentPlayer] = useState(null);
  const [isLoggingIn, setIsLoggingIn] = useState(false);

  const fetchQuestions = () => {
    return getQuestions()
      .then((data) => setQuestions(data))
      .catch((error) => console.error("Error fetching questions:", error));
  };

  const fetchPlayers = () => {
    return getPlayers()
      .then((data) => setPlayers(data))
      .catch((error) => console.error("Error fetching players:", error));
  };

  useEffect(() => {
    fetchQuestions();
    fetchPlayers();
  }, []);

  const handleLogin = async ({ username }) => {
    setIsLoggingIn(true);
    try {
      const playersList = await getPlayers();
      setPlayers(playersList);

      const matchingPlayer = playersList.find(
        (player) => player?.username?.toLowerCase() === username.toLowerCase(),
      );

      if (matchingPlayer) {
        setCurrentPlayer(matchingPlayer);
        return;
      }

      const createdPlayer = await createPlayer(username);
      setCurrentPlayer(createdPlayer);
      fetchPlayers();
    } catch (error) {
      console.error("Login failed:", error);
    } finally {
      setIsLoggingIn(false);
    }
  };

  const handleLogout = () => {
    setCurrentPlayer(null);
  };

  if (!currentPlayer) {
    return <LoginPage onLogin={handleLogin} isLoading={isLoggingIn} />;
  }

  return (
    <div className="min-h-screen bg-red-100">
      <div className="flex items-center justify-between px-6 py-4">
        <div>
          <div className="text-sm text-gray-500">Logged in as</div>
          <div className="text-lg font-semibold text-cyan-700">
            {currentPlayer.username}
          </div>
        </div>
        <button
          className="rounded-lg border border-cyan-200 px-4 py-2 text-sm font-semibold text-cyan-700 transition hover:border-cyan-300 hover:bg-cyan-50"
          onClick={handleLogout}
        >
          Log out
        </button>
      </div>
      <div className="grid gap-6 px-6 pb-8 lg:grid-cols-[2fr_1fr]">
        <QuizGame
          questions={questions}
          onRestart={fetchQuestions}
          playerId={currentPlayer.playerId}
          players={players}
          onRefreshPlayers={fetchPlayers}
        />
        <Leaderboard players={players} onRefresh={fetchPlayers} />
      </div>
    </div>
  );
}

export default App;
