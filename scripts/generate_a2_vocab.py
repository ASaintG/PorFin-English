#!/usr/bin/env python3
"""Generate A2 English-Spanish vocabulary seed JSON."""
from __future__ import annotations

import json
import sys
from pathlib import Path

DEFAULT_OUTPUT = (
    Path(__file__).resolve().parents[1]
    / "apps/api/InglesApp.Infrastructure/Seed/a2-vocabulary.json"
)

# (word, translation, partOfSpeech, category)
TRAVEL = [
    ("passport", "pasaporte", "noun", "travel"),
    ("luggage", "equipaje", "noun", "travel"),
    ("suitcase", "maleta", "noun", "travel"),
    ("ticket", "billete", "noun", "travel"),
    ("boarding pass", "tarjeta de embarque", "phrase", "travel"),
    ("customs", "aduanas", "noun", "travel"),
    ("border", "frontera", "noun", "travel"),
    ("destination", "destino", "noun", "travel"),
    ("journey", "viaje", "noun", "travel"),
    ("trip", "excursión", "noun", "travel"),
    ("tourist", "turista", "noun", "travel"),
    ("guide", "guía", "noun", "travel"),
    ("map", "mapa", "noun", "travel"),
    ("hotel", "hotel", "noun", "travel"),
    ("reservation", "reserva", "noun", "travel"),
    ("reception", "recepción", "noun", "travel"),
    ("check in", "registrarse", "phrase", "travel"),
    ("check out", "hacer el check-out", "phrase", "travel"),
    ("hostel", "albergue", "noun", "travel"),
    ("airport", "aeropuerto", "noun", "travel"),
    ("terminal", "terminal", "noun", "travel"),
    ("gate", "puerta de embarque", "noun", "travel"),
    ("flight", "vuelo", "noun", "travel"),
    ("delay", "retraso", "noun", "travel"),
    ("departure", "salida", "noun", "travel"),
    ("arrival", "llegada", "noun", "travel"),
    ("platform", "andén", "noun", "travel"),
    ("train", "tren", "noun", "travel"),
    ("bus station", "estación de autobuses", "phrase", "travel"),
    ("taxi", "taxi", "noun", "travel"),
    ("fare", "tarifa", "noun", "travel"),
    ("highway", "autopista", "noun", "travel"),
    ("bridge", "puente", "noun", "travel"),
    ("ferry", "ferry", "noun", "travel"),
    ("cruise", "crucero", "noun", "travel"),
    ("beach", "playa", "noun", "travel"),
    ("mountain", "montaña", "noun", "travel"),
    ("landscape", "paisaje", "noun", "travel"),
    ("souvenir", "recuerdo", "noun", "travel"),
    ("currency", "moneda", "noun", "travel"),
    ("exchange rate", "tipo de cambio", "phrase", "travel"),
    ("travel insurance", "seguro de viaje", "phrase", "travel"),
    ("lost and found", "objetos perdidos", "phrase", "travel"),
    ("single room", "habitación individual", "phrase", "travel"),
    ("double room", "habitación doble", "phrase", "travel"),
    ("backpack", "mochila", "noun", "travel"),
    ("visa", "visado", "noun", "travel"),
    ("landmark", "punto de referencia", "noun", "travel"),
    ("itinerary", "itinerario", "noun", "travel"),
    ("abroad", "en el extranjero", "adverb", "travel"),
]

WORK = [
    ("colleague", "colega", "noun", "work"),
    ("employer", "empleador", "noun", "work"),
    ("employee", "empleado", "noun", "work"),
    ("salary", "salario", "noun", "work"),
    ("wage", "sueldo", "noun", "work"),
    ("contract", "contrato", "noun", "work"),
    ("deadline", "fecha límite", "noun", "work"),
    ("meeting", "reunión", "noun", "work"),
    ("interview", "entrevista", "noun", "work"),
    ("application", "solicitud", "noun", "work"),
    ("CV", "currículum", "noun", "work"),
    ("resume", "currículum", "noun", "work"),
    ("promotion", "ascenso", "noun", "work"),
    ("overtime", "horas extra", "noun", "work"),
    ("shift", "turno", "noun", "work"),
    ("full time", "a tiempo completo", "phrase", "work"),
    ("part time", "a tiempo parcial", "phrase", "work"),
    ("workplace", "lugar de trabajo", "noun", "work"),
    ("office", "oficina", "noun", "work"),
    ("department", "departamento", "noun", "work"),
    ("manager", "gerente", "noun", "work"),
    ("supervisor", "supervisor", "noun", "work"),
    ("team", "equipo", "noun", "work"),
    ("project", "proyecto", "noun", "work"),
    ("task", "tarea", "noun", "work"),
    ("report", "informe", "noun", "work"),
    ("email", "correo electrónico", "noun", "work"),
    ("schedule", "horario", "noun", "work"),
    ("appointment", "cita", "noun", "work"),
    ("client", "cliente", "noun", "work"),
    ("customer", "cliente", "noun", "work"),
    ("invoice", "factura", "noun", "work"),
    ("expense", "gasto", "noun", "work"),
    ("budget", "presupuesto", "noun", "work"),
    ("training", "formación", "noun", "work"),
    ("skill", "habilidad", "noun", "work"),
    ("experience", "experiencia", "noun", "work"),
    ("qualification", "titulación", "noun", "work"),
    ("unemployment", "desempleo", "noun", "work"),
    ("retirement", "jubilación", "noun", "work"),
    ("resign", "dimitir", "verb", "work"),
    ("hire", "contratar", "verb", "work"),
    ("fire", "despedir", "verb", "work"),
    ("attend", "asistir", "verb", "work"),
    ("organize", "organizar", "verb", "work"),
    ("communicate", "comunicar", "verb", "work"),
    ("responsible", "responsable", "adjective", "work"),
    ("professional", "profesional", "adjective", "work"),
    ("busy", "ocupado", "adjective", "work"),
    ("stressful", "estresante", "adjective", "work"),
]

HEALTH = [
    ("doctor", "médico", "noun", "health"),
    ("nurse", "enfermera", "noun", "health"),
    ("patient", "paciente", "noun", "health"),
    ("hospital", "hospital", "noun", "health"),
    ("clinic", "clínica", "noun", "health"),
    ("pharmacy", "farmacia", "noun", "health"),
    ("medicine", "medicina", "noun", "health"),
    ("pill", "pastilla", "noun", "health"),
    ("prescription", "receta médica", "noun", "health"),
    ("symptom", "síntoma", "noun", "health"),
    ("fever", "fiebre", "noun", "health"),
    ("cough", "tos", "noun", "health"),
    ("headache", "dolor de cabeza", "noun", "health"),
    ("stomachache", "dolor de estómago", "noun", "health"),
    ("pain", "dolor", "noun", "health"),
    ("injury", "lesión", "noun", "health"),
    ("allergy", "alergia", "noun", "health"),
    ("infection", "infección", "noun", "health"),
    ("vaccine", "vacuna", "noun", "health"),
    ("appointment", "cita médica", "noun", "health"),
    ("blood pressure", "presión arterial", "phrase", "health"),
    ("blood test", "análisis de sangre", "phrase", "health"),
    ("X-ray", "radiografía", "noun", "health"),
    ("surgery", "cirugía", "noun", "health"),
    ("emergency", "urgencia", "noun", "health"),
    ("ambulance", "ambulancia", "noun", "health"),
    ("dentist", "dentista", "noun", "health"),
    ("toothache", "dolor de muelas", "noun", "health"),
    ("healthy", "saludable", "adjective", "health"),
    ("sick", "enfermo", "adjective", "health"),
    ("ill", "enfermo", "adjective", "health"),
    ("tired", "cansado", "adjective", "health"),
    ("weak", "débil", "adjective", "health"),
    ("strong", "fuerte", "adjective", "health"),
    ("recover", "recuperarse", "verb", "health"),
    ("exercise", "ejercicio", "noun", "health"),
    ("diet", "dieta", "noun", "health"),
    ("nutrition", "nutrición", "noun", "health"),
    ("stress", "estrés", "noun", "health"),
    ("mental health", "salud mental", "phrase", "health"),
    ("rest", "descanso", "noun", "health"),
    ("sleep", "sueño", "noun", "health"),
    ("insomnia", "insomnio", "noun", "health"),
    ("bandage", "vendaje", "noun", "health"),
    ("thermometer", "termómetro", "noun", "health"),
    ("feel better", "sentirse mejor", "phrase", "health"),
    ("take care", "cuidarse", "phrase", "health"),
    ("broken bone", "hueso roto", "phrase", "health"),
    ("sore throat", "dolor de garganta", "phrase", "health"),
    ("feel sick", "sentirse mal", "phrase", "health"),
]

HOBBIES = [
    ("hobby", "pasatiempo", "noun", "hobbies"),
    ("interest", "interés", "noun", "hobbies"),
    ("photography", "fotografía", "noun", "hobbies"),
    ("painting", "pintura", "noun", "hobbies"),
    ("drawing", "dibujo", "noun", "hobbies"),
    ("music", "música", "noun", "hobbies"),
    ("guitar", "guitarra", "noun", "hobbies"),
    ("piano", "piano", "noun", "hobbies"),
    ("dancing", "baile", "noun", "hobbies"),
    ("singing", "canto", "noun", "hobbies"),
    ("reading", "lectura", "noun", "hobbies"),
    ("writing", "escritura", "noun", "hobbies"),
    ("gardening", "jardinería", "noun", "hobbies"),
    ("cooking", "cocina", "noun", "hobbies"),
    ("baking", "repostería", "noun", "hobbies"),
    ("knitting", "tejido", "noun", "hobbies"),
    ("sewing", "costura", "noun", "hobbies"),
    ("fishing", "pesca", "noun", "hobbies"),
    ("camping", "camping", "noun", "hobbies"),
    ("hiking", "senderismo", "noun", "hobbies"),
    ("cycling", "ciclismo", "noun", "hobbies"),
    ("swimming", "natación", "noun", "hobbies"),
    ("running", "running", "noun", "hobbies"),
    ("yoga", "yoga", "noun", "hobbies"),
    ("chess", "ajedrez", "noun", "hobbies"),
    ("board game", "juego de mesa", "phrase", "hobbies"),
    ("video game", "videojuego", "noun", "hobbies"),
    ("collecting", "coleccionismo", "noun", "hobbies"),
    ("stamp", "sello", "noun", "hobbies"),
    ("coin", "moneda", "noun", "hobbies"),
    ("craft", "manualidad", "noun", "hobbies"),
    ("volunteering", "voluntariado", "noun", "hobbies"),
    ("club", "club", "noun", "hobbies"),
    ("concert", "concierto", "noun", "hobbies"),
    ("museum", "museo", "noun", "hobbies"),
    ("theater", "teatro", "noun", "hobbies"),
    ("movie", "película", "noun", "hobbies"),
    ("series", "serie", "noun", "hobbies"),
    ("podcast", "podcast", "noun", "hobbies"),
    ("blog", "blog", "noun", "hobbies"),
    ("practice", "practicar", "verb", "hobbies"),
    ("enjoy", "disfrutar", "verb", "hobbies"),
    ("relax", "relajarse", "verb", "hobbies"),
    ("creative", "creativo", "adjective", "hobbies"),
    ("fun", "divertido", "adjective", "hobbies"),
    ("boring", "aburrido", "adjective", "hobbies"),
    ("free time", "tiempo libre", "phrase", "hobbies"),
    ("spare time", "tiempo libre", "phrase", "hobbies"),
    ("take up", "empezar a practicar", "phrase", "hobbies"),
    ("give up", "dejar de", "phrase", "hobbies"),
]

PAST_EVENTS = [
    ("yesterday", "ayer", "adverb", "past_events"),
    ("last week", "la semana pasada", "phrase", "past_events"),
    ("last month", "el mes pasado", "phrase", "past_events"),
    ("last year", "el año pasado", "phrase", "past_events"),
    ("ago", "hace", "adverb", "past_events"),
    ("once", "una vez", "adverb", "past_events"),
    ("twice", "dos veces", "adverb", "past_events"),
    ("already", "ya", "adverb", "past_events"),
    ("yet", "todavía", "adverb", "past_events"),
    ("just", "acabar de", "adverb", "past_events"),
    ("recently", "recientemente", "adverb", "past_events"),
    ("suddenly", "de repente", "adverb", "past_events"),
    ("finally", "finalmente", "adverb", "past_events"),
    ("eventually", "al final", "adverb", "past_events"),
    ("memory", "recuerdo", "noun", "past_events"),
    ("childhood", "infancia", "noun", "past_events"),
    ("past", "pasado", "noun", "past_events"),
    ("history", "historia", "noun", "past_events"),
    ("event", "evento", "noun", "past_events"),
    ("celebration", "celebración", "noun", "past_events"),
    ("wedding", "boda", "noun", "past_events"),
    ("graduation", "graduación", "noun", "past_events"),
    ("accident", "accidente", "noun", "past_events"),
    ("mistake", "error", "noun", "past_events"),
    ("success", "éxito", "noun", "past_events"),
    ("failure", "fracaso", "noun", "past_events"),
    ("visited", "visitó", "verb", "past_events"),
    ("traveled", "viajó", "verb", "past_events"),
    ("moved", "se mudó", "verb", "past_events"),
    ("met", "conoció", "verb", "past_events"),
    ("married", "se casó", "verb", "past_events"),
    ("graduated", "se graduó", "verb", "past_events"),
    ("won", "ganó", "verb", "past_events"),
    ("lost", "perdió", "verb", "past_events"),
    ("forgot", "olvidó", "verb", "past_events"),
    ("remembered", "recordó", "verb", "past_events"),
    ("happened", "pasó", "verb", "past_events"),
    ("changed", "cambió", "verb", "past_events"),
    ("started", "empezó", "verb", "past_events"),
    ("finished", "terminó", "verb", "past_events"),
    ("used to", "solía", "phrase", "past_events"),
    ("when I was young", "cuando era joven", "phrase", "past_events"),
    ("in those days", "en aquellos días", "phrase", "past_events"),
    ("long ago", "hace mucho tiempo", "phrase", "past_events"),
    ("at that time", "en aquel momento", "phrase", "past_events"),
    ("first time", "primera vez", "phrase", "past_events"),
    ("never before", "nunca antes", "phrase", "past_events"),
    ("old photo", "foto antigua", "phrase", "past_events"),
    ("childhood friend", "amigo de la infancia", "phrase", "past_events"),
    ("used to live", "vivía antes", "phrase", "past_events"),
]

OPINIONS = [
    ("think", "pensar", "verb", "opinions"),
    ("believe", "creer", "verb", "opinions"),
    ("agree", "estar de acuerdo", "verb", "opinions"),
    ("disagree", "no estar de acuerdo", "verb", "opinions"),
    ("prefer", "preferir", "verb", "opinions"),
    ("suppose", "suponer", "verb", "opinions"),
    ("feel", "sentir", "verb", "opinions"),
    ("hope", "esperar", "verb", "opinions"),
    ("wonder", "preguntarse", "verb", "opinions"),
    ("opinion", "opinión", "noun", "opinions"),
    ("idea", "idea", "noun", "opinions"),
    ("view", "punto de vista", "noun", "opinions"),
    ("point of view", "punto de vista", "phrase", "opinions"),
    ("argument", "argumento", "noun", "opinions"),
    ("reason", "razón", "noun", "opinions"),
    ("fact", "hecho", "noun", "opinions"),
    ("truth", "verdad", "noun", "opinions"),
    ("lie", "mentira", "noun", "opinions"),
    ("doubt", "duda", "noun", "opinions"),
    ("certain", "seguro", "adjective", "opinions"),
    ("sure", "seguro", "adjective", "opinions"),
    ("uncertain", "inseguro", "adjective", "opinions"),
    ("possible", "posible", "adjective", "opinions"),
    ("impossible", "imposible", "adjective", "opinions"),
    ("probably", "probablemente", "adverb", "opinions"),
    ("maybe", "quizás", "adverb", "opinions"),
    ("perhaps", "tal vez", "adverb", "opinions"),
    ("definitely", "definitivamente", "adverb", "opinions"),
    ("absolutely", "absolutamente", "adverb", "opinions"),
    ("exactly", "exactamente", "adverb", "opinions"),
    ("in my opinion", "en mi opinión", "phrase", "opinions"),
    ("I think that", "creo que", "phrase", "opinions"),
    ("I believe that", "creo que", "phrase", "opinions"),
    ("in my view", "desde mi punto de vista", "phrase", "opinions"),
    ("to be honest", "siendo honesto", "phrase", "opinions"),
    ("to tell the truth", "a decir verdad", "phrase", "opinions"),
    ("on the other hand", "por otro lado", "phrase", "opinions"),
    ("however", "sin embargo", "conjunction", "opinions"),
    ("although", "aunque", "conjunction", "opinions"),
    ("because", "porque", "conjunction", "opinions"),
    ("therefore", "por lo tanto", "adverb", "opinions"),
    ("instead", "en cambio", "adverb", "opinions"),
    ("rather", "más bien", "adverb", "opinions"),
    ("better", "mejor", "adjective", "opinions"),
    ("worse", "peor", "adjective", "opinions"),
    ("important", "importante", "adjective", "opinions"),
    ("necessary", "necesario", "adjective", "opinions"),
    ("useful", "útil", "adjective", "opinions"),
    ("useless", "inútil", "adjective", "opinions"),
    ("fair", "justo", "adjective", "opinions"),
    ("unfair", "injusto", "adjective", "opinions"),
    ("right", "correcto", "adjective", "opinions"),
    ("wrong", "incorrecto", "adjective", "opinions"),
]

CATEGORIES = {
    "travel",
    "work",
    "health",
    "hobbies",
    "past_events",
    "opinions",
}


def example_sentence(word: str, translation: str, pos: str, category: str) -> str:
    w = word
    templates = {
        "travel": f"We need our {w} before we leave.",
        "work": f"At work we often discuss the {w}.",
        "health": f"The doctor asked about my {w}.",
        "hobbies": f"In my free time I enjoy {w}.",
        "past_events": f"Last year something related to {w} happened to me.",
        "opinions": f"In my opinion, {w} matters a lot.",
    }
    if pos == "phrase":
        return f"People often say \"{w}\" in this situation."
    if pos == "preposition":
        return f"The keys are {w} the bag."
    if pos == "conjunction":
        return f"I wanted to go, {w} it was too late."
    if pos == "determiner":
        return f"{w.capitalize()} answer seems correct."
    if pos == "pronoun":
        return f"{w.capitalize()} is what I mean."
    if pos == "interjection":
        return f"She said \"{w}!\" with surprise."
    if pos == "adverb":
        if category == "past_events":
            return f"It happened {w}."
        if category == "opinions":
            return f"I {w} agree with you."
        return f"He spoke {w} about the trip."
    if pos == "verb":
        if category == "opinions":
            return f"I {w} this is a good idea."
        if category == "work":
            return f"We {w} at the office every week."
        if category == "past_events":
            return f"She {w} when she was a student."
        if category == "hobbies":
            return f"I {w} every weekend."
        if category == "health":
            return f"You should {w} if you feel unwell."
        return f"They {w} during the holiday."
    if pos == "adjective":
        return f"That seems very {w} to me."
    return templates.get(category, f"This is an example with \"{w}\" ({translation}).")


def build_entries() -> list[dict]:
    raw: list[tuple[str, str, str, str]] = []
    raw.extend(TRAVEL)
    raw.extend(WORK)
    raw.extend(HEALTH)
    raw.extend(HOBBIES)
    raw.extend(PAST_EVENTS)
    raw.extend(OPINIONS)

    seen: set[str] = set()
    entries: list[dict] = []
    for word, translation, pos, category in raw:
        key = word.strip().lower()
        if key in seen:
            continue
        seen.add(key)
        if category not in CATEGORIES:
            continue
        entries.append(
            {
                "word": word,
                "translation": translation,
                "partOfSpeech": pos,
                "category": category,
                "exampleSentences": [example_sentence(word, translation, pos, category)],
            }
        )
    return entries


def main() -> int:
    output = DEFAULT_OUTPUT
    if len(sys.argv) > 1:
        output = Path(sys.argv[1])
    entries = build_entries()
    if len(entries) < 200:
        raise SystemExit(f"Expected at least 200 entries, got {len(entries)}")
    output.parent.mkdir(parents=True, exist_ok=True)
    with output.open("w", encoding="utf-8") as f:
        json.dump(entries, f, ensure_ascii=False, indent=2)
        f.write("\n")
    print(len(entries))
    return len(entries)


if __name__ == "__main__":
    main()
    sys.exit(0)
