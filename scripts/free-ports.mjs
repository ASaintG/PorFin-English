import { execSync } from "node:child_process";

const ports = [5000];

for (const port of ports) {
  try {
    const pids = execSync(`lsof -ti :${port}`, { encoding: "utf8" })
      .trim()
      .split("\n")
      .filter(Boolean);

    for (const pid of pids) {
      console.log(`Liberando puerto ${port} (PID ${pid})`);
      process.kill(Number(pid), "SIGTERM");
    }
  } catch {
    // lsof returns non-zero when nothing is listening
  }
}
