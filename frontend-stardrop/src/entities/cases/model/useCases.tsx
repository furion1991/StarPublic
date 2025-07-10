'use client'

import { useQuery } from '@tanstack/react-query'

import { getAllCases } from '../api/cases'

type UseCasesProps = {
  page?: number
  pageItems?: number
}

export const useCases = ({ page, pageItems }: UseCasesProps) => {
  return useQuery({
    queryKey: ['cases', page, pageItems],
    queryFn: () => getAllCases({ page, pageItems })
  })
}
