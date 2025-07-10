'use client'

import { useQuery } from '@tanstack/react-query'

import { getCase } from '../api/cases'

type UseCaseProps = {
  id: string
}

export const useCase = ({ id }: UseCaseProps) => {
  return useQuery({
    queryKey: ['cases', id],
    queryFn: () => getCase(id)
  })
}
