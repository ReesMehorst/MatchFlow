type Props = {
    page: number;
    total: number;
    pageSize: number;
    canPrev: boolean;
    canNext: boolean;
    onPrev: () => void;
    onNext: () => void;
};

export default function TeamsPager({
    page,
    total,
    pageSize,
    canPrev,
    canNext,
    onPrev,
    onNext,
}: Props) {
    return (
        <div className="teamsPager" aria-label="Pagination">
            <button
                className="btn btnSecondary"
                type="button"
                disabled={!canPrev}
                onClick={onPrev}
            >
                Prev
            </button>

            <span className="pagerText">
                Page {page} • {total} total
                {pageSize ? "" : ""}
            </span>

            <button
                className="btn btnSecondary"
                type="button"
                disabled={!canNext}
                onClick={onNext}
            >
                Next
            </button>
        </div>
    );
}