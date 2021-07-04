import {
	createContext,
	ReactNode,
	useContext,
	useEffect,
	useState,
} from "react";

interface PageTitleContextProps {
	title: string;
	setTitle: (title: string) => void;
}

export const PageTitleContext = createContext<PageTitleContextProps>({
	title: "",
	setTitle: () => {},
});

export function PageTitleProvider({ children }: { children: ReactNode }) {
	const title = usePageTitleProvider();

	return (
		<PageTitleContext.Provider value={title}>
			{children}
		</PageTitleContext.Provider>
	);
}

function usePageTitleProvider(): PageTitleContextProps {
	const [title, setTitle] = useState("");

	return {
		title,
		setTitle,
	};
}

export function usePageTitle(
	newTitle?: string,
	retainOnUnmount: boolean = false
) {
	const { title, setTitle } = useContext(PageTitleContext);

	useEffect(() => {
		if (newTitle) {
			setTitle(newTitle);
		}

		return () => {
			if (!retainOnUnmount) {
				setTitle("");
			}
		};
	}, []);

	return newTitle ?? title;
}
