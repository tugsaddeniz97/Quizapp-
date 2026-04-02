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
