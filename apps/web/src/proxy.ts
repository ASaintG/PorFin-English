import { NextResponse } from "next/server";
import type { NextRequest } from "next/server";

const publicRoutes = ["/login", "/register"];

export function proxy(request: NextRequest) {
  const { pathname } = request.nextUrl;
  if (publicRoutes.some((r) => pathname.startsWith(r))) return NextResponse.next();

  const token = request.cookies.get("token")?.value;
  if (!token) {
    const loginUrl = new URL("/login", request.url);
    loginUrl.searchParams.set("redirect", pathname);
    return NextResponse.redirect(loginUrl);
  }
  return NextResponse.next();
}

export const config = {
  matcher: [
    "/dashboard/:path*",
    "/review/:path*",
    "/lessons/:path*",
    "/grammar/:path*",
    "/reading/:path*",
    "/listening/:path*",
    "/speaking/:path*",
    "/writing/:path*",
    "/path/:path*",
    "/exam/:path*",
    "/onboarding/:path*",
    "/settings/:path*",
  ],
};
