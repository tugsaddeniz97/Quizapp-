const BASE_URL = "https://localhost:7239/";

export async function getQuestions() {
  const response = await fetch(`${BASE_URL}play/questions`);
  if (!response.ok) {
    throw new Error(`HTTP error! status: ${response.status}`);
  }
  const data = await response.json();
  console.log("Fetched questions:", data);
  return data;
}

export async function submitAnswer({
  playerId,
  questionId,
  answer,
  timeTakenSeconds,
}) {
  const response = await fetch(`${BASE_URL}play`, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify({
      PlayerId: playerId,
      QuestionId: questionId,
      Answer: answer,
      TimeTakenSeconds: timeTakenSeconds,
    }),
  });

  if (!response.ok) {
    throw new Error(`HTTP error! status: ${response.status}`);
  }

  return response.json();
}

export async function getPlayers() {
  const response = await fetch(`${BASE_URL}Player`);
  if (!response.ok) {
    throw new Error(`HTTP error! status: ${response.status}`);
  }
  return response.json();
}

export async function createPlayer(username) {
  const response = await fetch(`${BASE_URL}Player`, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify({
      userName: username,
    }),
  });

  if (!response.ok) {
    throw new Error(`HTTP error! status: ${response.status}`);
  }

  return response.json();
}
