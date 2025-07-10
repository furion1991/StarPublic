import { CasePreview } from '../CasePreview/CasePreview'

export const HeroicBundlesCasesList = () => {
  const casesList = Array.from({ length: 10 })
    .fill(null)
    .map((_, idx) => ({
      id: idx,
      name: 'Мрачный ронин',
      price: Math.floor(Math.random() * 10_000),
      oldPrice: Math.floor(Math.random() * 100_000)
    }))

  return (
    <>
      {casesList.map(({ id, name, price, oldPrice }) => {
        return (
          <CasePreview
            key={id}
            name={name}
            price={price}
            oldPrice={oldPrice}
            openLimit={1000}
            openedCasesNumber={0}
          />
        )
      })}
    </>
  )
}
