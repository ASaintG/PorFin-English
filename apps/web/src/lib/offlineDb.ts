const DB_NAME = "inglesapp";
const DB_VERSION = 2;
const REVIEW_QUEUE_STORE = "reviewQueue";
const SESSION_STORE = "reviewSession";
const READING_STORE = "readingTexts";
const LISTENING_STORE = "listeningExercises";

export interface PendingReview {
  id: string;
  itemId: string;
  quality: number;
  createdAt: string;
}

export interface CachedReviewSession {
  items: unknown[];
  cachedAt: string;
}

function openDb(): Promise<IDBDatabase> {
  return new Promise((resolve, reject) => {
    const request = indexedDB.open(DB_NAME, DB_VERSION);

    request.onerror = () => reject(request.error);
    request.onsuccess = () => resolve(request.result);

    request.onupgradeneeded = () => {
      const db = request.result;
      if (!db.objectStoreNames.contains(REVIEW_QUEUE_STORE)) {
        db.createObjectStore(REVIEW_QUEUE_STORE, { keyPath: "id" });
      }
      if (!db.objectStoreNames.contains(SESSION_STORE)) {
        db.createObjectStore(SESSION_STORE, { keyPath: "id" });
      }
      if (!db.objectStoreNames.contains(READING_STORE)) {
        db.createObjectStore(READING_STORE, { keyPath: "id" });
      }
      if (!db.objectStoreNames.contains(LISTENING_STORE)) {
        db.createObjectStore(LISTENING_STORE, { keyPath: "id" });
      }
    };
  });
}

async function withStore<T>(
  storeName: string,
  mode: IDBTransactionMode,
  fn: (store: IDBObjectStore) => IDBRequest<T>,
): Promise<T> {
  const db = await openDb();
  return new Promise((resolve, reject) => {
    const tx = db.transaction(storeName, mode);
    const store = tx.objectStore(storeName);
    const request = fn(store);
    request.onsuccess = () => resolve(request.result);
    request.onerror = () => reject(request.error);
  });
}

export async function enqueueReview(itemId: string, quality: number): Promise<void> {
  const entry: PendingReview = {
    id: `${itemId}-${Date.now()}`,
    itemId,
    quality,
    createdAt: new Date().toISOString(),
  };
  await withStore(REVIEW_QUEUE_STORE, "readwrite", (store) => store.put(entry));
}

export async function getPendingReviews(): Promise<PendingReview[]> {
  return withStore<PendingReview[]>(REVIEW_QUEUE_STORE, "readonly", (store) => store.getAll());
}

export async function removePendingReview(id: string): Promise<void> {
  await withStore(REVIEW_QUEUE_STORE, "readwrite", (store) => store.delete(id));
}

export async function cacheReviewSession(items: unknown[]): Promise<void> {
  const entry: CachedReviewSession & { id: string } = {
    id: "current",
    items,
    cachedAt: new Date().toISOString(),
  };
  await withStore(SESSION_STORE, "readwrite", (store) => store.put(entry));
}

export async function getCachedReviewSession(): Promise<CachedReviewSession | null> {
  const result = await withStore<CachedReviewSession & { id: string } | undefined>(
    SESSION_STORE,
    "readonly",
    (store) => store.get("current"),
  );
  return result ?? null;
}

export async function cacheReadingText(id: string, data: unknown): Promise<void> {
  await withStore(READING_STORE, "readwrite", (store) => store.put({ id, data, cachedAt: new Date().toISOString() }));
}

export async function getCachedReadingText(id: string): Promise<unknown | null> {
  const result = await withStore<{ data: unknown } | undefined>(READING_STORE, "readonly", (store) => store.get(id));
  return result?.data ?? null;
}

export async function cacheListeningExercise(id: string, data: unknown): Promise<void> {
  await withStore(LISTENING_STORE, "readwrite", (store) => store.put({ id, data, cachedAt: new Date().toISOString() }));
}

export async function getCachedListeningExercise(id: string): Promise<unknown | null> {
  const result = await withStore<{ data: unknown } | undefined>(LISTENING_STORE, "readonly", (store) => store.get(id));
  return result?.data ?? null;
}
