'use client'

import Link from 'next/link'

import { CasePreview } from '../CasePreview/CasePreview'

import { useCases } from '../../model/useCases'

export const PopularCasesList = () => {
  const { data: casesData, isLoading: isCasesLoading } = useCases({
    // page: 1,
    // pageItems: 4
  })

  return (
    <>
      {casesData?.cases?.map(
        ({ id, name, image, price, oldPrice, currentOpen, openLimit, type }) => {
          return (
            <Link key={id} href={`/cases/${id}`}>
              <CasePreview
                name={name}
                image={image}
                imageType={type}
                price={price}
                oldPrice={oldPrice}
                openLimit={openLimit}
                openedCasesNumber={currentOpen}
              />
            </Link>
          )
        }
      )}
    </>
  )
}
