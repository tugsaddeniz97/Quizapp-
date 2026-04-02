import { useState, useEffect } from "react";
import { getQuestions } from "./services/api";
import "./App.css";
import LandingPage from "./components/LandingPage.jsx";
import QuizGame from "./components/QuizGame.jsx";
function App() {
  const [questions, setQuestions] = useState([]);

  const fetchQuestions = () => {
    return getQuestions()
      .then((data) => setQuestions(data))
      .catch((error) => console.error("Error fetching questions:", error));
  };

  useEffect(() => {
    fetchQuestions();
  }, []);

  return (
    <div>
      <QuizGame questions={questions} onRestart={fetchQuestions} />
    </div>
  );
}

export default App;
