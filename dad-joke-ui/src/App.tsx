import React, { useState } from "react";
import { Input, Button, Card, Typography, Spin, Row, Col, message } from "antd";
import axios from "axios";

const { Title, Text } = Typography;

type JokeLengthGroup = "Short" | "Medium" | "Long";

interface Joke {
  id: string;
  joke: string;
  group?: JokeLengthGroup;
}

interface JokeGroup {
  group: JokeLengthGroup;
  jokes: Joke[];
}

const App: React.FC = () => {
  const [term, setTerm] = useState("");
  const [loading, setLoading] = useState(false);
  const [joke, setJoke] = useState<Joke | null>(null);
  const [jokeGroups, setJokeGroups] = useState<JokeGroup[]>([]);

  const classifyByLength = (text: string): JokeLengthGroup => {
    const wordCount = text.split(" ").length;
    if (wordCount < 10) return "Short";
    if (wordCount < 20) return "Medium";
    return "Long";
  };

  const groupJokes = (jokes: Joke[]): JokeGroup[] => {
    const grouped: Record<JokeLengthGroup, Joke[]> = {
      Short: [],
      Medium: [],
      Long: [],
    };

    for (const joke of jokes) {
      const group = joke.group || classifyByLength(joke.joke);
      grouped[group].push(joke);
    }

    return Object.entries(grouped)
      .map(([group, jokes]) => ({
        group: group as JokeLengthGroup,
        jokes,
      }))
      .filter(g => g.jokes.length > 0);
  };

  const highlight = (text: string, term: string): string => {
    const safeTerm = term.replace(/[.*+?^${}()|[\]\\]/g, "\\$&"); // escape regex
    const regex = new RegExp(`(${safeTerm})`, "gi");
    return text.replace(regex, "<mark>$1</mark>");
  };

  const handleRandom = async () => {
    setLoading(true);
    setJoke(null);
    setJokeGroups([]);

    try {
      const res = await axios.get<Joke>("https://localhost:7245/api/joke/random");
      if (!res.data || !res.data.joke) {
        message.warning("No joke returned from server.");
        return;
      }

      const singleJoke = res.data;
      singleJoke.group = classifyByLength(singleJoke.joke);

      setJoke(singleJoke);
      setJokeGroups(groupJokes([singleJoke]));
    } catch (error) {
      console.error("Error fetching random joke:", error);
      message.error("Error fetching random joke.");
    } finally {
      setLoading(false);
    }
  };

 const handleSearch = async () => {
  if (!term.trim()) {
    message.warning("Please enter a search term.");
    return;
  }

  setLoading(true);
  setJoke(null);
  setJokeGroups([]);

  try {
    const res = await axios.get<JokeGroup[]>(
      `https://localhost:7245/api/joke/search?term=${encodeURIComponent(term)}`
    );

    // ✅ Inject highlight into jokes inside each group
    const updatedGroups = res.data.map(group => ({
      ...group,
      jokes: group.jokes.map(j => ({
        ...j,
        joke: highlight(j.joke, term),
      })),
    }));

    setJokeGroups(updatedGroups);
  } catch (error) {
    console.error("Error searching jokes:", error);
    message.error("Error searching jokes.");
  } finally {
    setLoading(false);
  }
};


  return (
    <div style={{ maxWidth: 800, margin: "2rem auto", padding: "1rem" }}>
      <Title level={2}>😂 Dad Joke Explorer</Title>
      <Input.Group compact>
        <Input
          style={{ width: "calc(100% - 220px)" }}
          placeholder="Enter search term (e.g. cat)"
          value={term}
          onChange={(e) => setTerm(e.target.value)}
        />
        <Button type="primary" onClick={handleSearch}>
          Search
        </Button>
        <Button onClick={handleRandom} style={{ marginLeft: 8 }}>
          Random
        </Button>
      </Input.Group>

      <div style={{ marginTop: "2rem" }}>
        {loading ? (
          <Spin size="large" />
        ) : jokeGroups.length > 0 ? (
          jokeGroups.map((group) => (
            <div key={group.group} style={{ marginBottom: "2rem" }}>
              <Title level={4}>{group.group} Jokes</Title>
              <Row gutter={[16, 16]}>
                {group.jokes.map((j) => (
                  <Col span={24} key={j.id}>
                    <Card>
                      <div
                        dangerouslySetInnerHTML={{
                          __html: j.joke,
                        }}
                      />
                    </Card>
                  </Col>
                ))}
              </Row>
            </div>
          ))
        ) : joke ? (
          <Card>
            <Text>{joke.joke}</Text>
          </Card>
        ) : (
          <Text>No jokes found.</Text>
        )}
      </div>
    </div>
  );
};

export default App;
