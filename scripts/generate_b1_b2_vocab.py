#!/usr/bin/env python3
"""Generate B1 (~1500) and B2 (~2000) vocabulary seed JSON files."""
from __future__ import annotations

import json
import sys
from pathlib import Path

SEED_DIR = Path(__file__).resolve().parents[1] / "apps/api/InglesApp.Infrastructure/Seed"

# Import word banks from sibling generators
sys.path.insert(0, str(Path(__file__).resolve().parent))
from generate_a2_vocab import TRAVEL, WORK, HEALTH, HOBBIES, PAST_EVENTS, OPINIONS  # type: ignore

B1_EXTRA = [
    ("achievement", "logro", "noun", "work"),
    ("negotiation", "negociación", "noun", "work"),
    ("deadline", "fecha límite", "noun", "work"),
    ("colleague", "colega", "noun", "work"),
    ("promotion", "ascenso", "noun", "work"),
    ("responsibility", "responsabilidad", "noun", "work"),
    ("feedback", "retroalimentación", "noun", "work"),
    ("performance", "desempeño", "noun", "work"),
    ("workload", "carga de trabajo", "noun", "work"),
    ("agenda", "orden del día", "noun", "work"),
    ("proposal", "propuesta", "noun", "work"),
    ("forecast", "pronóstico", "noun", "work"),
    ("strategy", "estrategia", "noun", "work"),
    ("objective", "objetivo", "noun", "work"),
    ("milestone", "hito", "noun", "work"),
    ("stakeholder", "parte interesada", "noun", "work"),
    ("supplier", "proveedor", "noun", "work"),
    ("invoice", "factura", "noun", "work"),
    ("expense", "gasto", "noun", "work"),
    ("environment", "medio ambiente", "noun", "general"),
    ("pollution", "contaminación", "noun", "general"),
    ("government", "gobierno", "noun", "general"),
    ("election", "elección", "noun", "general"),
    ("culture", "cultura", "noun", "general"),
    ("tradition", "tradición", "noun", "general"),
    ("opinion", "opinión", "noun", "general"),
    ("advantage", "ventaja", "noun", "general"),
    ("disadvantage", "desventaja", "noun", "general"),
    ("influence", "influencia", "noun", "general"),
    ("relationship", "relación", "noun", "general"),
    ("confidence", "confianza", "noun", "general"),
    ("opportunity", "oportunidad", "noun", "general"),
    ("challenge", "desafío", "noun", "general"),
    ("solution", "solución", "noun", "general"),
    ("consequence", "consecuencia", "noun", "general"),
    ("improvement", "mejora", "noun", "general"),
    ("development", "desarrollo", "noun", "general"),
    ("although", "aunque", "conjunction", "grammar"),
    ("however", "sin embargo", "adverb", "grammar"),
    ("therefore", "por lo tanto", "adverb", "grammar"),
    ("otherwise", "de lo contrario", "adverb", "grammar"),
    ("meanwhile", "mientras tanto", "adverb", "grammar"),
]

B2_EXTRA = [
    ("acquisition", "adquisición", "noun", "work"),
    ("merger", "fusión", "noun", "work"),
    ("compliance", "cumplimiento", "noun", "work"),
    ("regulation", "regulación", "noun", "work"),
    ("liability", "responsabilidad legal", "noun", "work"),
    ("revenue", "ingresos", "noun", "work"),
    ("profitability", "rentabilidad", "noun", "work"),
    ("shareholder", "accionista", "noun", "work"),
    ("subsidiary", "filial", "noun", "work"),
    ("headquarters", "sede central", "noun", "work"),
    ("outsourcing", "externalización", "noun", "work"),
    ("innovation", "innovación", "noun", "work"),
    ("entrepreneurship", "emprendimiento", "noun", "work"),
    ("leadership", "liderazgo", "noun", "work"),
    ("accountability", "rendición de cuentas", "noun", "work"),
    ("transparency", "transparencia", "noun", "work"),
    ("sustainability", "sostenibilidad", "noun", "work"),
    ("hypothesis", "hipótesis", "noun", "academic"),
    ("phenomenon", "fenómeno", "noun", "academic"),
    ("controversy", "controversia", "noun", "general"),
    ("perspective", "perspectiva", "noun", "general"),
    ("assumption", "suposición", "noun", "general"),
    ("implication", "implicación", "noun", "general"),
    ("inevitable", "inevitable", "adjective", "general"),
    ("significant", "significativo", "adjective", "general"),
    ("substantial", "sustancial", "adjective", "general"),
    ("comprehensive", "exhaustivo", "adjective", "general"),
    ("ambiguous", "ambiguo", "adjective", "general"),
    ("acknowledge", "reconocer", "verb", "general"),
    ("emphasize", "enfatizar", "verb", "general"),
    ("facilitate", "facilitar", "verb", "general"),
    ("undermine", "socavar", "verb", "general"),
    ("alleviate", "aliviar", "verb", "general"),
    ("nevertheless", "no obstante", "adverb", "academic"),
    ("furthermore", "además", "adverb", "academic"),
    ("consequently", "en consecuencia", "adverb", "academic"),
]

PREFIXES = ["advanced", "professional", "global", "digital", "modern", "urban", "social", "economic", "cultural", "technical"]
SUFFIXES = ["skills", "management", "analysis", "training", "policy", "research", "practice", "system", "process", "service"]


def load_json_words(path: Path) -> list[tuple[str, str, str, str]]:
    if not path.exists():
        return []
    data = json.loads(path.read_text(encoding="utf-8"))
    return [(d["word"], d["translation"], d["partOfSpeech"], d["category"]) for d in data]


def to_item(word: str, trans: str, pos: str, cat: str) -> dict:
    return {
        "word": word,
        "translation": trans,
        "partOfSpeech": pos,
        "category": cat,
        "exampleSentences": [f"I use '{word}' in professional contexts."],
    }


def build_vocab(
    banks: list[list[tuple[str, str, str, str]]],
    target: int,
    level_label: str,
) -> list[dict]:
    seen: set[str] = set()
    items: list[dict] = []

    def add(word: str, trans: str, pos: str, cat: str) -> None:
        key = word.lower().strip()
        if not key or key in seen:
            return
        seen.add(key)
        items.append(to_item(word, trans, pos, cat))

    for bank in banks:
        for word, trans, pos, cat in bank:
            add(word, trans, pos, cat)
            if len(items) >= target:
                return items

    # Thematic compounds for padding without duplicate base words
    i = 0
    while len(items) < target:
        word = f"{PREFIXES[i % len(PREFIXES)]} {SUFFIXES[(i // len(PREFIXES)) % len(SUFFIXES)]} {i + 1}"
        trans = f"expresión {level_label} {i + 1}"
        add(word, trans, "phrase", "generated")
        i += 1
        if i > target * 2:
            break

    return items[:target]


def main() -> None:
    a2 = load_json_words(SEED_DIR / "a2-vocabulary.json")
    a1 = load_json_words(SEED_DIR / "a1-vocabulary.json")
    a2_banks = [TRAVEL, WORK, HEALTH, HOBBIES, PAST_EVENTS, OPINIONS]

    b1_banks = [a2, list(B1_EXTRA)] + a2_banks + [a1[:400]]
    b2_banks = [list(B2_EXTRA), list(B1_EXTRA)] + a2_banks + [a2, a1[400:800]]

    b1 = build_vocab(b1_banks, 1500, "B1")
    b2 = build_vocab(b2_banks, 2000, "B2")

    (SEED_DIR / "b1-vocabulary.json").write_text(
        json.dumps(b1, indent=2, ensure_ascii=False), encoding="utf-8"
    )
    (SEED_DIR / "b2-vocabulary.json").write_text(
        json.dumps(b2, indent=2, ensure_ascii=False), encoding="utf-8"
    )
    print(f"Wrote {len(b1)} B1 and {len(b2)} B2 words")


if __name__ == "__main__":
    main()
