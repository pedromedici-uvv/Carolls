import Logo from "./Logo";
import Search from "./Search";

export default function navbar() {
  return (
    <header className="sticky z-50 top-0 flex flex-row justify-between bg-white p-5 items-center text-gray-800 shadow-md">
      <Logo />
      <Search />
      <div>Login</div>
    </header>
  );
}
