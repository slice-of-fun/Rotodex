import os
from fastapi import FastAPI, HTTPException
from fastapi.middleware.cors import CORSMiddleware
from pydantic import BaseModel
import httpx
from dotenv import load_dotenv

# Load from the root .env file
load_dotenv("../../.env")

app = FastAPI()

# Allow requests from the Blazor WebAssembly frontend
app.add_middleware(
    CORSMiddleware,
    allow_origins=["*"], # In production, set this to your hosted Blazor app URL
    allow_credentials=True,
    allow_methods=["*"],
    allow_headers=["*"],
)

GROQ_API_KEY = os.getenv("GROQ_API_KEY")

class ExplainRequest(BaseModel):
    speciesName: str
    isLegal: bool
    checkMsg: str

class TeamAnalyzeRequest(BaseModel):
    showdownExport: str

@app.post("/api/ai/explain")
async def explain_legality(req: ExplainRequest):
    if not GROQ_API_KEY:
        raise HTTPException(status_code=500, detail="Groq API key not configured on server.")

    prompt = f"""
You are a Pokémon legality expert. I am analyzing a {req.speciesName}.
The legality checker returned the following result for this Pokémon:
Status: {"Valid" if req.isLegal else "Invalid"}
Message: {req.checkMsg}

Explain this legality check in 1-2 short, simple sentences. Don't use overly technical hexadecimal terms if possible. Make it easy for a casual player to understand why this is {"legal" if req.isLegal else "illegal"}.
"""

    return await call_groq(prompt, "You are a helpful Pokémon legality expert.", max_tokens=150)

@app.post("/api/ai/analyze-team")
async def analyze_team(req: TeamAnalyzeRequest):
    if not GROQ_API_KEY:
        raise HTTPException(status_code=500, detail="Groq API key not configured on server.")

    prompt = f"""
You are a competitive Pokémon team building expert. I am building a Pokémon team and here is my current team in Showdown format:

{req.showdownExport}

Please analyze this team for competitive battles. Provide:
1. A brief overview of the team's strengths and core strategy.
2. The team's biggest weaknesses (e.g. type weaknesses, lack of hazards, speed control).
3. 2-3 specific suggestions for improvement (e.g. swap a move, change an item, or suggest a specific Pokémon to replace an existing one to improve synergy).
Keep your response concise, using Markdown formatting, and strictly focused on competitive Pokémon mechanics.
"""

    return await call_groq(prompt, "You are a competitive Pokémon team building expert.", max_tokens=500)


async def call_groq(user_prompt: str, system_prompt: str, max_tokens: int):
    url = "https://api.groq.com/openai/v1/chat/completions"
    headers = {
        "Authorization": f"Bearer {GROQ_API_KEY}",
        "Content-Type": "application/json"
    }
    payload = {
        "model": "llama3-8b-8192",
        "messages": [
            {"role": "system", "content": system_prompt},
            {"role": "user", "content": user_prompt}
        ],
        "temperature": 0.4,
        "max_tokens": max_tokens
    }

    async with httpx.AsyncClient() as client:
        try:
            response = await client.post(url, json=payload, headers=headers, timeout=30.0)
            response.raise_for_status()
            data = response.json()
            return {"content": data["choices"][0]["message"]["content"].strip()}
        except Exception as e:
            raise HTTPException(status_code=502, detail=f"Failed to reach Groq API: {str(e)}")
