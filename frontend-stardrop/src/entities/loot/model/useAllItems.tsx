import { useQuery } from '@tanstack/react-query'

import { getAllItems } from '../api/loot'

type UseAllItemsProps = {
  valueFrom: number
  nameSearch: string
}

export const useAllItems = ({ nameSearch, valueFrom }: UseAllItemsProps) => {
  return useQuery({
    queryFn: () => getAllItems({ valueFrom, nameSearch }),
    queryKey: ['all-items', valueFrom, nameSearch]
  })
}
